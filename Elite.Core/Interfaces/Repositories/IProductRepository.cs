using Elite.Core.Common;
using Elite.Core.DTOs.Product;
using Elite.Core.Models;

namespace Elite.Core.Interfaces.Repositories;

public interface IProductRepository
{
    // Product CRUD
    Task<Product?> GetByIdAsync(int id, bool includeInactive = false);
    Task<Product?> GetBySlugAsync(string slug, bool includeInactive = false);
    Task<PagedResult<Product>> GetAllAsync(ProductFilterRequest filter);
    Task<Product> AddAsync(Product product);
    Task<Product> UpdateAsync(Product product);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);

    // Variant Operations
    Task<ProductVariant?> GetVariantByIdAsync(int variantId);
    Task<ProductVariant?> GetVariantBySkuAsync(string sku);
    Task<List<ProductVariant>> GetVariantsByProductIdAsync(int productId);
    Task<ProductVariant> AddVariantAsync(ProductVariant variant);
    Task<ProductVariant> UpdateVariantAsync(ProductVariant variant);
    Task<bool> DeleteVariantAsync(int variantId);
    Task<bool> SkuExistsAsync(string sku, int? excludeId = null);

    // Image Operations
    Task<ProductImage> AddImageAsync(ProductImage image);
    Task<List<ProductImage>> AddImagesAsync(List<ProductImage> images);
    Task<bool> DeleteImageAsync(int imageId);
    Task<ProductImage?> GetMainImageAsync(int productId);

    // Stock Operations
    Task<bool> UpdateStockAsync(int variantId, int quantity);
    Task<bool> DecrementStockAsync(int variantId, int quantity);
    Task<List<ProductVariant>> GetLowStockVariantsAsync(int threshold);

    // Featured & Active
    Task<List<Product>> GetFeaturedProductsAsync(int count = 10);
    Task<bool> ToggleFeaturedAsync(int productId);
    Task<bool> ToggleActiveAsync(int productId);
}