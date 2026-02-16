using Elite.Core.Common;
using Elite.Core.DTOs.Product;
using Elite.Core.Enums;
using Elite.Core.Interfaces.Repositories;
using Elite.Core.Models;
using Elite.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Elite.Infrastructure.Repositories;

public partial class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id, bool includeInactive = false)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .Include(p => p.Variants.OrderBy(v => v.SortOrder))
                .ThenInclude(v => v.VariantImages.OrderBy(vi => vi.SortOrder))
            .AsQueryable();

        if (!includeInactive)
            query = query.Where(p => p.IsActive);

        return await query.FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<Product?> GetBySlugAsync(string slug, bool includeInactive = false)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images.OrderBy(i => i.SortOrder))
            .Include(p => p.Variants.OrderBy(v => v.SortOrder))
                .ThenInclude(v => v.VariantImages.OrderBy(vi => vi.SortOrder))
            .AsQueryable();

        if (!includeInactive)
            query = query.Where(p => p.IsActive);

        return await query.FirstOrDefaultAsync(p => p.Slug == slug);
    }

    public async Task<Product> AddAsync(Product product)
    {
        await _context.Products.AddAsync(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<Product> UpdateAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        _context.Products.Update(product);
        await _context.SaveChangesAsync();
        return product;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product == null) return false;

        _context.Products.Remove(product);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
    {
        var query = _context.Products.Where(p => p.Slug == slug);
        if (excludeId.HasValue)
            query = query.Where(p => p.Id != excludeId.Value);

        return await query.AnyAsync();
    }
}

public partial class ProductRepository
{
    public async Task<PagedResult<Product>> GetAllAsync(ProductFilterRequest filter)
    {
        var query = _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images.Where(i => i.IsMain))
            .Include(p => p.Variants.Where(v => v.IsActive))
            .Where(p => p.IsActive)
            .AsQueryable();

        // Search by name or description
        if (!string.IsNullOrWhiteSpace(filter.SearchTerm))
        {
            var searchTerm = filter.SearchTerm.ToLower();
            query = query.Where(p =>
                p.Name.ToLower().Contains(searchTerm) ||
                (p.Description != null && p.Description.ToLower().Contains(searchTerm)) ||
                p.Brand.Name.ToLower().Contains(searchTerm)
            );
        }

        // Filter by category
        if (filter.CategoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == filter.CategoryId.Value);
        }

        // Filter by brand
        if (filter.BrandId.HasValue)
        {
            query = query.Where(p => p.BrandId == filter.BrandId.Value);
        }

        // Price range filter (applied on variants)
        if (filter.MinPrice.HasValue || filter.MaxPrice.HasValue)
        {
            query = query.Where(p => p.Variants.Any(v =>
                (!filter.MinPrice.HasValue || (p.BasePrice + v.AddonPrice) >= filter.MinPrice.Value) &&
                (!filter.MaxPrice.HasValue || (p.BasePrice + v.AddonPrice) <= filter.MaxPrice.Value)
            ));
        }

        // Laptop-specific filters
        if (filter.ProcessorBrands?.Any() == true)
        {
            query = query.Where(p => p.Variants.Any(v =>
                v.ProcessorBrand != null && filter.ProcessorBrands.Contains(v.ProcessorBrand)
            ));
        }

        if (filter.RamSizes?.Any() == true)
        {
            query = query.Where(p => p.Variants.Any(v =>
                v.RamSizeGB.HasValue && filter.RamSizes.Contains(v.RamSizeGB.Value)
            ));
        }

        if (filter.StorageCapacities?.Any() == true)
        {
            query = query.Where(p => p.Variants.Any(v =>
                v.StorageCapacityGB.HasValue && filter.StorageCapacities.Contains(v.StorageCapacityGB.Value)
            ));
        }

        if (filter.GpuTypes?.Any() == true)
        {
            query = query.Where(p => p.Variants.Any(v =>
                v.GpuType != null && filter.GpuTypes.Contains(v.GpuType)
            ));
        }

        if (filter.DisplaySizes?.Any() == true)
        {
            query = query.Where(p => p.Variants.Any(v =>
                v.DisplaySizeInches.HasValue && filter.DisplaySizes.Contains(v.DisplaySizeInches.Value)
            ));
        }

        // Condition filter
        if (filter.Conditions?.Any() == true)
        {
            query = query.Where(p => p.Variants.Any(v => filter.Conditions.Contains(v.Condition)));
        }

        // Stock status filter
        if (filter.StockStatuses?.Any() == true)
        {
            foreach (var status in filter.StockStatuses)
            {
                query = status switch
                {
                    StockStatus.InStock => query.Where(p => p.Variants.Any(v => v.StockQuantity > p.StockAlertThreshold)),
                    StockStatus.LowStock => query.Where(p => p.Variants.Any(v => v.StockQuantity > 0 && v.StockQuantity <= p.StockAlertThreshold)),
                    StockStatus.OutOfStock => query.Where(p => p.Variants.Any(v => v.StockQuantity == 0 && !p.AllowPreOrder)),
                    StockStatus.PreOrder => query.Where(p => p.AllowPreOrder && p.Variants.Any(v => v.StockQuantity == 0)),
                    _ => query
                };
            }
        }

        // Sorting
        query = filter.SortBy?.ToLower() switch
        {
            "price_low" => query.OrderBy(p => p.BasePrice + p.Variants.Min(v => v.AddonPrice)),
            "price_high" => query.OrderByDescending(p => p.BasePrice + p.Variants.Max(v => v.AddonPrice)),
            "name" => query.OrderBy(p => p.Name),
            "popular" => query.OrderByDescending(p => p.IsFeatured).ThenByDescending(p => p.CreatedAt),
            _ => query.OrderByDescending(p => p.CreatedAt) // newest (default)
        };

        var totalCount = await query.CountAsync();

        var products = await query
            .Skip((filter.PageNumber - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return new PagedResult<Product>
        {
            Items = products,
            TotalCount = totalCount,
            PageNumber = filter.PageNumber,
            PageSize = filter.PageSize
        };
    }

    public async Task<List<Product>> GetFeaturedProductsAsync(int count = 10)
    {
        return await _context.Products
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .Include(p => p.Images.Where(i => i.IsMain))
            .Include(p => p.Variants.Where(v => v.IsActive))
            .Where(p => p.IsActive && p.IsFeatured)
            .OrderByDescending(p => p.CreatedAt)
            .Take(count)
            .ToListAsync();
    }
}


public partial class ProductRepository
{
    // ========== VARIANT OPERATIONS ==========

    public async Task<ProductVariant?> GetVariantByIdAsync(int variantId)
    {
        return await _context.ProductVariants
            .Include(v => v.Product)
                .ThenInclude(p => p.Brand)
            .Include(v => v.Product)
                .ThenInclude(p => p.Category)
            .Include(v => v.VariantImages.OrderBy(vi => vi.SortOrder))
            .FirstOrDefaultAsync(v => v.Id == variantId);
    }

    public async Task<ProductVariant?> GetVariantBySkuAsync(string sku)
    {
        return await _context.ProductVariants
            .Include(v => v.Product)
            .FirstOrDefaultAsync(v => v.SKU == sku);
    }

    public async Task<List<ProductVariant>> GetVariantsByProductIdAsync(int productId)
    {
        return await _context.ProductVariants
            .Include(v => v.VariantImages.OrderBy(vi => vi.SortOrder))
            .Where(v => v.ProductId == productId)
            .OrderBy(v => v.SortOrder)
            .ToListAsync();
    }

    public async Task<ProductVariant> AddVariantAsync(ProductVariant variant)
    {
        await _context.ProductVariants.AddAsync(variant);
        await _context.SaveChangesAsync();
        return variant;
    }

    public async Task<ProductVariant> UpdateVariantAsync(ProductVariant variant)
    {
        variant.UpdatedAt = DateTime.UtcNow;
        _context.ProductVariants.Update(variant);
        await _context.SaveChangesAsync();
        return variant;
    }

    public async Task<bool> DeleteVariantAsync(int variantId)
    {
        var variant = await _context.ProductVariants.FindAsync(variantId);
        if (variant == null) return false;

        _context.ProductVariants.Remove(variant);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> SkuExistsAsync(string sku, int? excludeId = null)
    {
        var query = _context.ProductVariants.Where(v => v.SKU == sku);
        if (excludeId.HasValue)
            query = query.Where(v => v.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    // ========== IMAGE OPERATIONS ==========

    public async Task<ProductImage> AddImageAsync(ProductImage image)
    {
        await _context.ProductImages.AddAsync(image);
        await _context.SaveChangesAsync();
        return image;
    }

    public async Task<List<ProductImage>> AddImagesAsync(List<ProductImage> images)
    {
        await _context.ProductImages.AddRangeAsync(images);
        await _context.SaveChangesAsync();
        return images;
    }

    public async Task<bool> DeleteImageAsync(int imageId)
    {
        var image = await _context.ProductImages.FindAsync(imageId);
        if (image == null) return false;

        _context.ProductImages.Remove(image);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProductImage?> GetMainImageAsync(int productId)
    {
        return await _context.ProductImages
            .Where(i => i.ProductId == productId && i.IsMain)
            .FirstOrDefaultAsync();
    }

    // ========== STOCK OPERATIONS ==========

    public async Task<bool> UpdateStockAsync(int variantId, int quantity)
    {
        var variant = await _context.ProductVariants.FindAsync(variantId);
        if (variant == null) return false;

        variant.StockQuantity = quantity;
        variant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> DecrementStockAsync(int variantId, int quantity)
    {
        var variant = await _context.ProductVariants.FindAsync(variantId);
        if (variant == null) return false;

        variant.StockQuantity -= quantity;
        variant.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<List<ProductVariant>> GetLowStockVariantsAsync(int threshold)
    {
        return await _context.ProductVariants
            .Include(v => v.Product)
                .ThenInclude(p => p.Brand)
            .Where(v => v.IsActive && v.StockQuantity > 0 && v.StockQuantity <= threshold)
            .OrderBy(v => v.StockQuantity)
            .ToListAsync();
    }

    // ========== STATUS OPERATIONS ==========

    public async Task<bool> ToggleFeaturedAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return false;

        product.IsFeatured = !product.IsFeatured;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ToggleActiveAsync(int productId)
    {
        var product = await _context.Products.FindAsync(productId);
        if (product == null) return false;

        product.IsActive = !product.IsActive;
        product.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return true;
    }
}
