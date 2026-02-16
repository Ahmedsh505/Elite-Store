using Elite.Core.Common;
using Elite.Core.DTOs.Product;
using Elite.Core.Enums;
using Elite.Core.Interfaces.Repositories;
using Elite.Core.Interfaces.Services;
using Elite.Core.Models;

namespace Elite.Services.Services;

public partial class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBrandRepository _brandRepository;

    public ProductService(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IBrandRepository brandRepository)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _brandRepository = brandRepository;
    }

    public async Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request, int createdBy)
    {
        // Validate category exists
        if (!await _categoryRepository.ExistsAsync(request.CategoryId))
            return Result<ProductResponse>.Failure("Category not found");

        // Validate brand exists
        if (!await _brandRepository.ExistsAsync(request.BrandId))
            return Result<ProductResponse>.Failure("Brand not found");

        // Generate slug
        var slug = GenerateSlug(request.Name);
        var originalSlug = slug;
        var counter = 1;

        while (await _productRepository.SlugExistsAsync(slug))
        {
            slug = $"{originalSlug}-{counter++}";
        }

        var product = new Product
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ShortDescription = request.ShortDescription,
            CategoryId = request.CategoryId,
            BrandId = request.BrandId,
            BasePrice = request.BasePrice,
            CompareAtPrice = request.CompareAtPrice,
            AllowPreOrder = request.AllowPreOrder,
            StockAlertThreshold = request.StockAlertThreshold,
            IsFeatured = request.IsFeatured,
            MetaTitle = request.MetaTitle ?? request.Name,
            MetaDescription = request.MetaDescription,
            MetaKeywords = request.MetaKeywords,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        var createdProduct = await _productRepository.AddAsync(product);

        // reload with includes
        var fullProduct = await _productRepository.GetByIdAsync(createdProduct.Id, includeInactive: true);

        var response = await MapToProductResponse(fullProduct!);


        return Result<ProductResponse>.Success(response);
    }

    public async Task<Result<ProductResponse>> UpdateProductAsync(int id, CreateProductRequest request, int updatedBy)
    {
        var product = await _productRepository.GetByIdAsync(id, includeInactive: true);
        if (product == null)
            return Result<ProductResponse>.Failure("Product not found");

        // Validate category exists
        if (!await _categoryRepository.ExistsAsync(request.CategoryId))
            return Result<ProductResponse>.Failure("Category not found");

        // Validate brand exists
        if (!await _brandRepository.ExistsAsync(request.BrandId))
            return Result<ProductResponse>.Failure("Brand not found");

        // Update slug if name changed
        if (product.Name != request.Name)
        {
            var slug = GenerateSlug(request.Name);
            var originalSlug = slug;
            var counter = 1;

            while (await _productRepository.SlugExistsAsync(slug, excludeId: id))
            {
                slug = $"{originalSlug}-{counter++}";
            }
            product.Slug = slug;
        }

        product.Name = request.Name;
        product.Description = request.Description;
        product.ShortDescription = request.ShortDescription;
        product.CategoryId = request.CategoryId;
        product.BrandId = request.BrandId;
        product.BasePrice = request.BasePrice;
        product.CompareAtPrice = request.CompareAtPrice;
        product.AllowPreOrder = request.AllowPreOrder;
        product.StockAlertThreshold = request.StockAlertThreshold;
        product.IsFeatured = request.IsFeatured;
        product.MetaTitle = request.MetaTitle ?? request.Name;
        product.MetaDescription = request.MetaDescription;
        product.MetaKeywords = request.MetaKeywords;
        product.UpdatedBy = updatedBy;

        var updatedProduct = await _productRepository.UpdateAsync(product);

        var fullProduct = await _productRepository
            .GetByIdAsync(updatedProduct.Id, includeInactive: true);

        var response = await MapToProductResponse(fullProduct!);


        return Result<ProductResponse>.Success(response);
    }

    public async Task<Result> DeleteProductAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return Result.Failure("Product not found");

        // Check if product has orders (future implementation)
        // For now, just delete

        var deleted = await _productRepository.DeleteAsync(id);
        return deleted
            ? Result.Success()
            : Result.Failure("Failed to delete product");
    }

    public async Task<Result<ProductResponse>> GetProductByIdAsync(int id)
    {
        var product = await _productRepository.GetByIdAsync(id);
        if (product == null)
            return Result<ProductResponse>.Failure("Product not found");

        var response = await MapToProductResponse(product);
        return Result<ProductResponse>.Success(response);
    }

    public async Task<Result<ProductResponse>> GetProductBySlugAsync(string slug)
    {
        var product = await _productRepository.GetBySlugAsync(slug);
        if (product == null)
            return Result<ProductResponse>.Failure("Product not found");

        var response = await MapToProductResponse(product);
        return Result<ProductResponse>.Success(response);
    }

    // Helper method
    private string GenerateSlug(string name)
    {
        return name.ToLower()
            .Replace(" ", "-")
            .Replace("&", "and")
            .Replace("/", "-")
            .Replace("\\", "-")
            .Replace("'", "")
            .Replace("\"", "");
    }

    // ========== VARIANT MANAGEMENT ==========

    public async Task<Result<ProductVariantDto>> CreateVariantAsync(CreateProductVariantRequest request)
    {
        // Validate product exists
        if (!await _productRepository.ExistsAsync(request.ProductId))
            return Result<ProductVariantDto>.Failure("Product not found");

        // Validate SKU is unique
        if (await _productRepository.SkuExistsAsync(request.SKU))
            return Result<ProductVariantDto>.Failure($"SKU '{request.SKU}' already exists");

        var variant = new ProductVariant
        {
            ProductId = request.ProductId,
            SKU = request.SKU,
            ProcessorBrand = request.ProcessorBrand,
            ProcessorModel = request.ProcessorModel,
            ProcessorGeneration = request.ProcessorGeneration,
            ProcessorSpeed = request.ProcessorSpeed,
            RamSizeGB = request.RamSizeGB,
            RamType = request.RamType,
            RamSpeed = request.RamSpeed,
            StorageType = request.StorageType,
            StorageCapacityGB = request.StorageCapacityGB,
            StorageInterface = request.StorageInterface,
            GpuType = request.GpuType,
            GpuBrand = request.GpuBrand,
            GpuModel = request.GpuModel,
            GpuVramGB = request.GpuVramGB,
            DisplaySizeInches = request.DisplaySizeInches,
            DisplayResolution = request.DisplayResolution,
            DisplayRefreshRate = request.DisplayRefreshRate,
            DisplayPanelType = request.DisplayPanelType,
            OperatingSystem = request.OperatingSystem,
            Color = request.Color,
            WarrantyMonths = request.WarrantyMonths,
            ConnectionType = request.ConnectionType,
            Compatibility = request.Compatibility,
            AdditionalAttributesJson = request.AdditionalAttributesJson,
            AddonPrice = request.AddonPrice,
            CompareAtAddonPrice = request.CompareAtAddonPrice,
            Condition = request.Condition,
            StockQuantity = request.StockQuantity,
            IsDefault = request.IsDefault,
            SortOrder = request.SortOrder,
            CreatedAt = DateTime.UtcNow
        };

        var createdVariant = await _productRepository.AddVariantAsync(variant);

        // Reload with product details
        var variantWithProduct = await _productRepository.GetVariantByIdAsync(createdVariant.Id);
        var response = MapToVariantDto(variantWithProduct!);

        return Result<ProductVariantDto>.Success(response);
    }

    public async Task<Result<ProductVariantDto>> UpdateVariantAsync(int variantId, CreateProductVariantRequest request)
    {
        var variant = await _productRepository.GetVariantByIdAsync(variantId);
        if (variant == null)
            return Result<ProductVariantDto>.Failure("Variant not found");

        // Validate SKU is unique
        if (await _productRepository.SkuExistsAsync(request.SKU, excludeId: variantId))
            return Result<ProductVariantDto>.Failure($"SKU '{request.SKU}' already exists");

        variant.SKU = request.SKU;
        variant.ProcessorBrand = request.ProcessorBrand;
        variant.ProcessorModel = request.ProcessorModel;
        variant.ProcessorGeneration = request.ProcessorGeneration;
        variant.ProcessorSpeed = request.ProcessorSpeed;
        variant.RamSizeGB = request.RamSizeGB;
        variant.RamType = request.RamType;
        variant.RamSpeed = request.RamSpeed;
        variant.StorageType = request.StorageType;
        variant.StorageCapacityGB = request.StorageCapacityGB;
        variant.StorageInterface = request.StorageInterface;
        variant.GpuType = request.GpuType;
        variant.GpuBrand = request.GpuBrand;
        variant.GpuModel = request.GpuModel;
        variant.GpuVramGB = request.GpuVramGB;
        variant.DisplaySizeInches = request.DisplaySizeInches;
        variant.DisplayResolution = request.DisplayResolution;
        variant.DisplayRefreshRate = request.DisplayRefreshRate;
        variant.DisplayPanelType = request.DisplayPanelType;
        variant.OperatingSystem = request.OperatingSystem;
        variant.Color = request.Color;
        variant.WarrantyMonths = request.WarrantyMonths;
        variant.ConnectionType = request.ConnectionType;
        variant.Compatibility = request.Compatibility;
        variant.AdditionalAttributesJson = request.AdditionalAttributesJson;
        variant.AddonPrice = request.AddonPrice;
        variant.CompareAtAddonPrice = request.CompareAtAddonPrice;
        variant.Condition = request.Condition;
        variant.StockQuantity = request.StockQuantity;
        variant.IsDefault = request.IsDefault;
        variant.SortOrder = request.SortOrder;

        var updatedVariant = await _productRepository.UpdateVariantAsync(variant);
        var response = MapToVariantDto(updatedVariant);

        return Result<ProductVariantDto>.Success(response);
    }

    public async Task<Result> DeleteVariantAsync(int variantId)
    {
        var variant = await _productRepository.GetVariantByIdAsync(variantId);
        if (variant == null)
            return Result.Failure("Variant not found");

        // Future: Check if variant has orders

        var deleted = await _productRepository.DeleteVariantAsync(variantId);
        return deleted
            ? Result.Success()
            : Result.Failure("Failed to delete variant");
    }

    public async Task<Result<List<ProductVariantDto>>> GetVariantsByProductIdAsync(int productId)
    {
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
            return Result<List<ProductVariantDto>>.Failure("Product not found");

        var variants = product.Variants
            .Select(v => MapToVariantDto(v, product))
            .ToList();

        return Result<List<ProductVariantDto>>.Success(variants);
    }

    public async Task<Result<ProductVariantDto>> GetVariantByIdAsync(int variantId)
    {
        var variant = await _productRepository.GetVariantByIdAsync(variantId);
        if (variant == null)
            return Result<ProductVariantDto>.Failure("Variant not found");

        var response = MapToVariantDto(variant);
        return Result<ProductVariantDto>.Success(response);
    }

    // ========== LISTING & SEARCH ==========

    public async Task<Result<PagedResult<ProductListResponse>>> GetProductsAsync(ProductFilterRequest filter)
    {
        var pagedProducts = await _productRepository.GetAllAsync(filter);

        var responses = pagedProducts.Items.Select(p => MapToProductListResponse(p)).ToList();

        return Result<PagedResult<ProductListResponse>>.Success(new PagedResult<ProductListResponse>
        {
            Items = responses,
            TotalCount = pagedProducts.TotalCount,
            PageNumber = pagedProducts.PageNumber,
            PageSize = pagedProducts.PageSize
        });
    }

    public async Task<Result<List<ProductListResponse>>> GetFeaturedProductsAsync(int count = 10)
    {
        var products = await _productRepository.GetFeaturedProductsAsync(count);
        var responses = products.Select(p => MapToProductListResponse(p)).ToList();

        return Result<List<ProductListResponse>>.Success(responses);
    }

    // ========== IMAGE MANAGEMENT ==========

    public async Task<Result> UploadProductImagesAsync(int productId, List<string> imageUrls)
    {
        if (!await _productRepository.ExistsAsync(productId))
            return Result.Failure("Product not found");

        var images = imageUrls.Select((url, index) => new ProductImage
        {
            ProductId = productId,
            ImageUrl = url,
            ThumbnailUrl = url, // In production, generate thumbnail
            IsMain = index == 0, // First image is main
            SortOrder = index,
            UploadedAt = DateTime.UtcNow
        }).ToList();

        await _productRepository.AddImagesAsync(images);
        return Result.Success();
    }

    public async Task<Result> DeleteProductImageAsync(int imageId)
    {
        var deleted = await _productRepository.DeleteImageAsync(imageId);
        return deleted
            ? Result.Success()
            : Result.Failure("Image not found");
    }

    public async Task<Result> SetMainImageAsync(int productId, int imageId)
    {
        // Reset current main image
        var currentMain = await _productRepository.GetMainImageAsync(productId);
        if (currentMain != null)
        {
            currentMain.IsMain = false;
        }

        // This would need to be implemented in repository
        // For now, simplified
        return Result.Success();
    }

    // ========== STOCK MANAGEMENT ==========

    public async Task<Result> UpdateStockAsync(int variantId, int quantity)
    {
        var variant = await _productRepository.GetVariantByIdAsync(variantId);
        if (variant == null)
            return Result.Failure("Variant not found");

        if (quantity < 0)
            return Result.Failure("Stock quantity cannot be negative");

        var updated = await _productRepository.UpdateStockAsync(variantId, quantity);
        return updated
            ? Result.Success()
            : Result.Failure("Failed to update stock");
    }

    public async Task<Result<List<ProductVariantDto>>> GetLowStockVariantsAsync()
    {
        // Get low stock across all products
        var variants = await _productRepository.GetLowStockVariantsAsync(threshold: 5);

        var responses = variants.Select(v => MapToVariantDto(v)).ToList();
        return Result<List<ProductVariantDto>>.Success(responses);
    }

    // ========== PRODUCT STATUS ==========

    public async Task<Result> ToggleFeaturedAsync(int productId)
    {
        var toggled = await _productRepository.ToggleFeaturedAsync(productId);
        return toggled
            ? Result.Success()
            : Result.Failure("Product not found");
    }

    public async Task<Result> ToggleActiveAsync(int productId)
    {
        var toggled = await _productRepository.ToggleActiveAsync(productId);
        return toggled
            ? Result.Success()
            : Result.Failure("Product not found");
    }

    // ========== MAPPING HELPERS ==========

    private ProductVariantDto MapToVariantDto(ProductVariant variant, Product? product = null)
    {
        product ??= variant.Product;

        var finalPrice = product.BasePrice + variant.AddonPrice;
        var finalCompareAtPrice = product.CompareAtPrice.HasValue && variant.CompareAtAddonPrice.HasValue
            ? product.CompareAtPrice.Value + variant.CompareAtAddonPrice.Value
            : product.CompareAtPrice;

        var stockStatus = DetermineStockStatus(variant, product);

        return new ProductVariantDto
        {
            Id = variant.Id,
            SKU = variant.SKU,
            ProcessorBrand = variant.ProcessorBrand,
            ProcessorModel = variant.ProcessorModel,
            ProcessorGeneration = variant.ProcessorGeneration,
            ProcessorSpeed = variant.ProcessorSpeed,
            RamSizeGB = variant.RamSizeGB,
            RamType = variant.RamType,
            RamSpeed = variant.RamSpeed,
            StorageType = variant.StorageType,
            StorageCapacityGB = variant.StorageCapacityGB,
            StorageInterface = variant.StorageInterface,
            GpuType = variant.GpuType,
            GpuBrand = variant.GpuBrand,
            GpuModel = variant.GpuModel,
            GpuVramGB = variant.GpuVramGB,
            DisplaySizeInches = variant.DisplaySizeInches,
            DisplayResolution = variant.DisplayResolution,
            DisplayRefreshRate = variant.DisplayRefreshRate,
            DisplayPanelType = variant.DisplayPanelType,
            OperatingSystem = variant.OperatingSystem,
            Color = variant.Color,
            WarrantyMonths = variant.WarrantyMonths,
            ConnectionType = variant.ConnectionType,
            Compatibility = variant.Compatibility,
            AdditionalAttributesJson = variant.AdditionalAttributesJson,
            FinalPrice = finalPrice,
            FinalCompareAtPrice = finalCompareAtPrice,
            AddonPrice = variant.AddonPrice,
            Condition = variant.Condition,
            ConditionDisplay = variant.Condition.ToString(),
            StockQuantity = variant.StockQuantity,
            StockStatus = stockStatus,
            StockStatusDisplay = GetStockStatusMessage(variant, product, stockStatus),
            IsDefault = variant.IsDefault,
            IsActive = variant.IsActive,
            Images = variant.VariantImages.Select(vi => new ProductImageDto
            {
                Id = vi.Id,
                ImageUrl = vi.ImageUrl,
                ThumbnailUrl = vi.ThumbnailUrl,
                AltText = vi.AltText,
                SortOrder = vi.SortOrder
            }).ToList()
        };
    }

    private StockStatus DetermineStockStatus(ProductVariant variant, Product product)
    {
        if (variant.StockQuantity == 0)
            return product.AllowPreOrder ? StockStatus.PreOrder : StockStatus.OutOfStock;

        if (variant.StockQuantity <= product.StockAlertThreshold)
            return StockStatus.LowStock;

        return StockStatus.InStock;
    }

    private string GetStockStatusMessage(ProductVariant variant, Product product, StockStatus status)
    {
        return status switch
        {
            StockStatus.InStock => variant.StockQuantity <= product.StockAlertThreshold
                ? $"Only {variant.StockQuantity} left in stock"
                : "In Stock",
            StockStatus.LowStock => $"Only {variant.StockQuantity} left in stock",
            StockStatus.OutOfStock => "Out of Stock",
            StockStatus.PreOrder => "Pre-order Available",
            _ => "Unknown"
        };
    }

    private async Task<ProductResponse> MapToProductResponse(Product product)
    {
        return new ProductResponse
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            Description = product.Description,
            ShortDescription = product.ShortDescription,
            BasePrice = product.BasePrice,
            CompareAtPrice = product.CompareAtPrice,
            IsActive = product.IsActive,
            IsFeatured = product.IsFeatured,
            AllowPreOrder = product.AllowPreOrder,
            StockAlertThreshold = product.StockAlertThreshold,
            Category = new CategoryDto
            {
                Id = product.Category.Id,
                Name = product.Category.Name,
                Slug = product.Category.Slug,
                ParentCategoryId = product.Category.ParentCategoryId,
                ParentCategoryName = product.Category.ParentCategory?.Name
            },
            Brand = new BrandDto
            {
                Id = product.Brand.Id,
                Name = product.Brand.Name,
                Slug = product.Brand.Slug,
                LogoUrl = product.Brand.LogoUrl
            },
            Images = product.Images.Select(i => new ProductImageDto
            {
                Id = i.Id,
                ImageUrl = i.ImageUrl,
                ThumbnailUrl = i.ThumbnailUrl,
                AltText = i.AltText,
                IsMain = i.IsMain,
                SortOrder = i.SortOrder
            }).ToList(),
            Variants = product.Variants.Select(v => MapToVariantDto(v, product)).ToList(),
            CreatedAt = product.CreatedAt,
            UpdatedAt = product.UpdatedAt
        };
    }

    private ProductListResponse MapToProductListResponse(Product product)
    {
        var activeVariants = product.Variants.Where(v => v.IsActive).ToList();
        var inStockVariants = activeVariants.Where(v => v.StockQuantity > 0).ToList();

        // Handle case where there are no active variants
        if (!activeVariants.Any())
        {
            return new ProductListResponse
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                ShortDescription = product.ShortDescription,
                StartingPrice = product.BasePrice,
                CompareAtStartingPrice = product.CompareAtPrice,
                IsFeatured = product.IsFeatured,
                CategoryName = product.Category.Name,
                BrandName = product.Brand.Name,
                MainImageUrl = product.Images.FirstOrDefault(i => i.IsMain)?.ImageUrl,
                TotalVariants = 0,
                InStockVariants = 0,
                OverallStockStatus = StockStatus.Discontinued,
                AvailableConditions = new List<ProductCondition>(),
                CreatedAt = product.CreatedAt
            };
        }

        var startingPrice = product.BasePrice + activeVariants.Min(v => v.AddonPrice);
        var minCompareAtAddon = activeVariants.Min(v => v.CompareAtAddonPrice);
        var compareAtStartingPrice = product.CompareAtPrice.HasValue && minCompareAtAddon.HasValue
            ? product.CompareAtPrice.Value + minCompareAtAddon.Value
            : product.CompareAtPrice;

        var overallStockStatus = DetermineOverallStockStatus(product, activeVariants);

        return new ProductListResponse
        {
            Id = product.Id,
            Name = product.Name,
            Slug = product.Slug,
            ShortDescription = product.ShortDescription,
            StartingPrice = startingPrice,
            CompareAtStartingPrice = compareAtStartingPrice,
            IsFeatured = product.IsFeatured,
            CategoryName = product.Category.Name,
            BrandName = product.Brand.Name,
            MainImageUrl = product.Images.FirstOrDefault(i => i.IsMain)?.ImageUrl,
            TotalVariants = activeVariants.Count,
            InStockVariants = inStockVariants.Count,
            OverallStockStatus = overallStockStatus,
            AvailableConditions = activeVariants.Select(v => v.Condition).Distinct().ToList(),
            CreatedAt = product.CreatedAt
        };
    }

    private StockStatus DetermineOverallStockStatus(Product product, List<ProductVariant> variants)
    {
        if (!variants.Any())
            return StockStatus.Discontinued;

        var hasStock = variants.Any(v => v.StockQuantity > 0);

        if (!hasStock)
            return product.AllowPreOrder ? StockStatus.PreOrder : StockStatus.OutOfStock;

        var hasLowStock = variants.Any(v => v.StockQuantity > 0 && v.StockQuantity <= product.StockAlertThreshold);
        return hasLowStock ? StockStatus.LowStock : StockStatus.InStock;
    }
}