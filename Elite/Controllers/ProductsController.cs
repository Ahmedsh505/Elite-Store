using Elite.Core.DTOs.Product;
using Elite.Core.Interfaces.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elite.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
// [Authorize(Roles = "Admin,ProductManager")] // ✅ COMMENTED OUT - Uncomment when JWT is ready
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;

    public ProductsController(IProductService productService)
    {
        _productService = productService;
    }

    // ========== PRODUCT CRUD ==========

    [HttpPost]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        // Get userId from claims (simplified - adjust based on your JWT structure)
        var userId = 1; // Hardcoded for now

        var result = await _productService.CreateProductAsync(request, userId);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage, errors = result.Errors });

        return CreatedAtAction(nameof(GetProduct), new { id = result.Data!.Id }, result.Data);
    }

    [HttpGet("{id}")]
    [AllowAnonymous] // Public endpoint
    public async Task<IActionResult> GetProduct(int id)
    {
        var result = await _productService.GetProductByIdAsync(id);

        if (!result.IsSuccess)
            return NotFound(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetProductBySlug(string slug)
    {
        var result = await _productService.GetProductBySlugAsync(slug);

        if (!result.IsSuccess)
            return NotFound(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateProduct(int id, [FromBody] CreateProductRequest request)
    {
        var userId = 1; // Hardcoded for now

        var result = await _productService.UpdateProductAsync(id, request, userId);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage, errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProductAsync(id);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage });

        return NoContent();
    }

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetProducts([FromQuery] ProductFilterRequest filter)
    {
        var result = await _productService.GetProductsAsync(filter);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpGet("featured")]
    [AllowAnonymous]
    public async Task<IActionResult> GetFeaturedProducts([FromQuery] int count = 10)
    {
        var result = await _productService.GetFeaturedProductsAsync(count);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }

    // ========== VARIANT MANAGEMENT ==========

    [HttpPost("variants")]
    public async Task<IActionResult> CreateVariant([FromBody] CreateProductVariantRequest request)
    {
        var result = await _productService.CreateVariantAsync(request);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage, errors = result.Errors });

        return CreatedAtAction(nameof(GetVariant), new { variantId = result.Data!.Id }, result.Data);
    }

    [HttpGet("variants/{variantId}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVariant(int variantId)
    {
        var result = await _productService.GetVariantByIdAsync(variantId);

        if (!result.IsSuccess)
            return NotFound(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpGet("{productId}/variants")]
    [AllowAnonymous]
    public async Task<IActionResult> GetVariantsByProduct(int productId)
    {
        var result = await _productService.GetVariantsByProductIdAsync(productId);

        if (!result.IsSuccess)
            return NotFound(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }

    [HttpPut("variants/{variantId}")]
    public async Task<IActionResult> UpdateVariant(int variantId, [FromBody] CreateProductVariantRequest request)
    {
        var result = await _productService.UpdateVariantAsync(variantId, request);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage, errors = result.Errors });

        return Ok(result.Data);
    }

    [HttpDelete("variants/{variantId}")]
    public async Task<IActionResult> DeleteVariant(int variantId)
    {
        var result = await _productService.DeleteVariantAsync(variantId);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage });

        return NoContent();
    }

    // ========== IMAGE MANAGEMENT ==========

    [HttpPost("{productId}/images")]
    public async Task<IActionResult> UploadImages(int productId, [FromBody] List<string> imageUrls)
    {
        var result = await _productService.UploadProductImagesAsync(productId, imageUrls);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(new { message = "Images uploaded successfully" });
    }

    [HttpDelete("images/{imageId}")]
    public async Task<IActionResult> DeleteImage(int imageId)
    {
        var result = await _productService.DeleteProductImageAsync(imageId);

        if (!result.IsSuccess)
            return NotFound(new { error = result.ErrorMessage });

        return NoContent();
    }

    // ========== STOCK MANAGEMENT ==========

    [HttpPut("variants/{variantId}/stock")]
    public async Task<IActionResult> UpdateStock(int variantId, [FromBody] int quantity)
    {
        var result = await _productService.UpdateStockAsync(variantId, quantity);

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(new { message = "Stock updated successfully" });
    }

    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStockVariants()
    {
        var result = await _productService.GetLowStockVariantsAsync();

        if (!result.IsSuccess)
            return BadRequest(new { error = result.ErrorMessage });

        return Ok(result.Data);
    }

    // ========== PRODUCT STATUS ==========

    [HttpPatch("{id}/toggle-featured")]
    public async Task<IActionResult> ToggleFeatured(int id)
    {
        var result = await _productService.ToggleFeaturedAsync(id);

        if (!result.IsSuccess)
            return NotFound(new { error = result.ErrorMessage });

        return Ok(new { message = "Featured status toggled" });
    }

    [HttpPatch("{id}/toggle-active")]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var result = await _productService.ToggleActiveAsync(id);

        if (!result.IsSuccess)
            return NotFound(new { error = result.ErrorMessage });

        return Ok(new { message = "Active status toggled" });
    }
}