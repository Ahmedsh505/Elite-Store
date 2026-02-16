using Elite.Core.Interfaces.Repositories;
using Elite.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Elite.Controllers;

[ApiController]
[Route("api/admin/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _categoryRepository;

    public CategoriesController(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    // ========== CATEGORY CRUD ==========

    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> GetAllCategories([FromQuery] bool includeInactive = false)
    {
        var categories = await _categoryRepository.GetAllAsync(includeInactive);
        return Ok(categories);
    }

    [HttpGet("root")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRootCategories()
    {
        var categories = await _categoryRepository.GetRootCategoriesAsync();
        return Ok(categories);
    }

    [HttpGet("{id}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategory(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return NotFound(new { error = "Category not found" });

        return Ok(category);
    }

    [HttpGet("slug/{slug}")]
    [AllowAnonymous]
    public async Task<IActionResult> GetCategoryBySlug(string slug)
    {
        var category = await _categoryRepository.GetBySlugAsync(slug);

        if (category == null)
            return NotFound(new { error = "Category not found" });

        return Ok(category);
    }

    [HttpGet("{parentId}/subcategories")]
    [AllowAnonymous]
    public async Task<IActionResult> GetSubCategories(int parentId)
    {
        var subCategories = await _categoryRepository.GetSubCategoriesAsync(parentId);
        return Ok(subCategories);
    }

    [HttpPost]
    [AllowAnonymous] // ✅ Changed: No auth required for development
    // [Authorize(Roles = "Admin,ProductManager")] // TODO: Uncomment when JWT is implemented
    public async Task<IActionResult> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        // Validate parent category exists if specified
        if (request.ParentCategoryId.HasValue)
        {
            if (!await _categoryRepository.ExistsAsync(request.ParentCategoryId.Value))
                return BadRequest(new { error = "Parent category not found" });
        }

        // Generate slug
        var slug = GenerateSlug(request.Name);
        var originalSlug = slug;
        var counter = 1;

        while (await _categoryRepository.SlugExistsAsync(slug))
        {
            slug = $"{originalSlug}-{counter++}";
        }

        var category = new Category
        {
            Name = request.Name,
            Slug = slug,
            Description = request.Description,
            ParentCategoryId = request.ParentCategoryId,
            SortOrder = request.SortOrder,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _categoryRepository.AddAsync(category);
        return CreatedAtAction(nameof(GetCategory), new { id = created.Id }, created);
    }

    [HttpPut("{id}")]
    [AllowAnonymous] // ✅ Changed: No auth required for development
    // [Authorize(Roles = "Admin,ProductManager")] // TODO: Uncomment when JWT is implemented
    public async Task<IActionResult> UpdateCategory(int id, [FromBody] CreateCategoryRequest request)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return NotFound(new { error = "Category not found" });

        // Validate parent category exists if specified
        if (request.ParentCategoryId.HasValue)
        {
            if (!await _categoryRepository.ExistsAsync(request.ParentCategoryId.Value))
                return BadRequest(new { error = "Parent category not found" });

            // Prevent circular reference
            if (request.ParentCategoryId.Value == id)
                return BadRequest(new { error = "Category cannot be its own parent" });
        }

        // Update slug if name changed
        if (category.Name != request.Name)
        {
            var slug = GenerateSlug(request.Name);
            var originalSlug = slug;
            var counter = 1;

            while (await _categoryRepository.SlugExistsAsync(slug, excludeId: id))
            {
                slug = $"{originalSlug}-{counter++}";
            }
            category.Slug = slug;
        }

        category.Name = request.Name;
        category.Description = request.Description;
        category.ParentCategoryId = request.ParentCategoryId;
        category.SortOrder = request.SortOrder;

        var updated = await _categoryRepository.UpdateAsync(category);
        return Ok(updated);
    }

    [HttpDelete("{id}")]
    [AllowAnonymous] // ✅ Changed: No auth required for development
    // [Authorize(Roles = "Admin")] // TODO: Uncomment when JWT is implemented
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var category = await _categoryRepository.GetByIdAsync(id);

        if (category == null)
            return NotFound(new { error = "Category not found" });

        // Check if category has products
        if (await _categoryRepository.HasProductsAsync(id))
            return BadRequest(new { error = "Cannot delete category with products. Remove products first." });

        // Check if category has subcategories
        var subCategories = await _categoryRepository.GetSubCategoriesAsync(id);
        if (subCategories.Any())
            return BadRequest(new { error = "Cannot delete category with subcategories. Remove subcategories first." });

        var deleted = await _categoryRepository.DeleteAsync(id);

        if (!deleted)
            return BadRequest(new { error = "Failed to delete category" });

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
public class CreateCategoryRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int? ParentCategoryId { get; set; }
    public int SortOrder { get; set; } = 0;
}