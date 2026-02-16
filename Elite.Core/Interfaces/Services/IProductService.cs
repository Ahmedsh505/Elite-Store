using Elite.Core.Common;
using Elite.Core.DTOs.Product;

namespace Elite.Core.Interfaces.Services;

public interface IProductService
{
    // Product Management
    Task<Result<ProductResponse>> CreateProductAsync(CreateProductRequest request, int createdBy);
    Task<Result<ProductResponse>> UpdateProductAsync(int id, CreateProductRequest request, int updatedBy);
    Task<Result> DeleteProductAsync(int id);
    Task<Result<ProductResponse>> GetProductByIdAsync(int id);
    Task<Result<ProductResponse>> GetProductBySlugAsync(string slug);
    Task<Result<PagedResult<ProductListResponse>>> GetProductsAsync(ProductFilterRequest filter);
    Task<Result<List<ProductListResponse>>> GetFeaturedProductsAsync(int count = 10);

    // Variant Management
    Task<Result<ProductVariantDto>> CreateVariantAsync(CreateProductVariantRequest request);
    Task<Result<ProductVariantDto>> UpdateVariantAsync(int variantId, CreateProductVariantRequest request);
    Task<Result> DeleteVariantAsync(int variantId);
    Task<Result<List<ProductVariantDto>>> GetVariantsByProductIdAsync(int productId);
    Task<Result<ProductVariantDto>> GetVariantByIdAsync(int variantId);

    // Image Management
    Task<Result> UploadProductImagesAsync(int productId, List<string> imageUrls);
    Task<Result> DeleteProductImageAsync(int imageId);
    Task<Result> SetMainImageAsync(int productId, int imageId);

    // Stock Management
    Task<Result> UpdateStockAsync(int variantId, int quantity);
    Task<Result<List<ProductVariantDto>>> GetLowStockVariantsAsync();

    // Product Status
    Task<Result> ToggleFeaturedAsync(int productId);
    Task<Result> ToggleActiveAsync(int productId);
}