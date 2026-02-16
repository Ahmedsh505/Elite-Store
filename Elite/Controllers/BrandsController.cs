using Elite.Core.Interfaces.Repositories;
using Elite.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elite.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
public class BrandsController : ControllerBase
{
    private readonly IBrandRepository _brandRepository;

    public BrandsController(IBrandRepository brandRepository)
    {
        _brandRepository = brandRepository;
    }

    // ========== BRAND CRUD ==========

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllBrands([FromQuery] bool includeInactive = false)
    {
        var brands = await _brandRepository.GetAllAsync(includeInactive);
        return Ok(brands);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBrand(int id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);

        if (brand == null)
            return NotFound(new { error = "Brand not found" });

        return Ok(brand);
    }

    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetBrandBySlug(string slug)
    {
        var brand = await _brandRepository.GetBySlugAsync(slug);

        if (brand == null)
            return NotFound(new { error = "Brand not found" });

        return Ok(brand);
    }

    [HttpPost]
    [AllowAnonymous] // ✅ Changed: No auth required for development
    // [Authorize(Roles = "Admin,ProductManager")] // TODO: Uncomment when JWT is implemented
    public async Task<IActionResult> CreateBrand([FromBody] CreateBrandRequest request)
    {
        // Generate slug
        var slug = GenerateSlug(request.Name);
        var originalSlug = slug;
        var counter = 1;

        while (await _brandRepository.SlugExistsAsync(slug))
        {
            slug = $"{originalSlug}-{counter++}";
        }

        var brand = new Brand
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            LogoUrl = request.LogoUrl,
            SortOrder = request.SortOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _brandRepository.AddAsync(brand);
        return CreatedAtAction(nameof(GetBrand), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [AllowAnonymous] // ✅ Changed: No auth required for development
    // [Authorize(Roles = "Admin,ProductManager")] // TODO: Uncomment when JWT is implemented
    public async Task<IActionResult> UpdateBrand(int id, [FromBody] CreateBrandRequest request)
    {
        var brand = await _brandRepository.GetByIdAsync(id);

        if (brand == null)
            return NotFound(new { error = "Brand not found" });

        // Update slug if name changed
        if (brand.Name != request.Name)
        {
            var slug = GenerateSlug(request.Name);
            var originalSlug = slug;
            var counter = 1;

            while (await _brandRepository.SlugExistsAsync(slug, excludeId: id))
            {
                slug = $"{originalSlug}-{counter++}";
            }
            brand.Slug = slug;
        }

        brand.Name = request.Name;
        brand.Description = request.Description;
        brand.LogoUrl = request.LogoUrl;
        brand.SortOrder = request.SortOrder;

        var updated = await _brandRepository.UpdateAsync(brand);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [AllowAnonymous] // ✅ Changed: No auth required for development
    // [Authorize(Roles = "Admin")] // TODO: Uncomment when JWT is implemented
    public async Task<IActionResult> DeleteBrand(int id)
    {
        var brand = await _brandRepository.GetByIdAsync(id);

        if (brand == null)
            return NotFound(new { error = "Brand not found" });

        // Check if brand has products
        if (await _brandRepository.HasProductsAsync(id))
            return BadRequest(new { error = "Cannot delete brand with products. Remove products first." });

        var deleted = await _brandRepository.DeleteAsync(id);

        if (!deleted)
            return BadRequest(new { error = "Failed to delete brand" });

        return NoContent();
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
}

// ========== DTO ==========
public class CreateBrandRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public int SortOrder { get; set; } = 0;
}