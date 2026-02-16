using Elite.Core.Interfaces.Repositories;
using Elite.Core.Models;
using Elite.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Elite.Infrastructure.Repositories;

public class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Category?> GetByIdAsync(int id)
    {
        return await _context.Categories
            .Include(c => c.SubCategories) // Only load direct children, not parent
            .FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<Category?> GetBySlugAsync(string slug)
    {
        return await _context.Categories
            .Include(c => c.SubCategories)
            .FirstOrDefaultAsync(c => c.Slug == slug);
    }

    public async Task<List<Category>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Categories
            .AsNoTracking() // ✅ Added: Improves performance and avoids tracking
            .AsQueryable();

        if (!includeInactive)
            query = query.Where(c => c.IsActive);

        // ✅ Don't include navigation properties to avoid circular reference
        return await query
            .OrderBy(c => c.SortOrder)
            .ThenBy(c => c.Name)
            .ToListAsync();
    }

    public async Task<List<Category>> GetRootCategoriesAsync()
    {
        return await _context.Categories
            .AsNoTracking()
            .Include(c => c.SubCategories.Where(sc => sc.IsActive)) // Only load active subcategories
            .Where(c => c.IsActive && c.ParentCategoryId == null)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
    }

    public async Task<List<Category>> GetSubCategoriesAsync(int parentId)
    {
        return await _context.Categories
            .AsNoTracking()
            .Where(c => c.IsActive && c.ParentCategoryId == parentId)
            .OrderBy(c => c.SortOrder)
            .ToListAsync();
    }

    public async Task<Category> AddAsync(Category category)
    {
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<Category> UpdateAsync(Category category)
    {
        category.UpdatedAt = DateTime.UtcNow;
        _context.Categories.Update(category);
        await _context.SaveChangesAsync();
        return category;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var category = await _context.Categories.FindAsync(id);
        if (category == null) return false;

        _context.Categories.Remove(category);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Categories.AnyAsync(c => c.Id == id);
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
    {
        var query = _context.Categories.Where(c => c.Slug == slug);
        if (excludeId.HasValue)
            query = query.Where(c => c.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> HasProductsAsync(int categoryId)
    {
        return await _context.Products.AnyAsync(p => p.CategoryId == categoryId);
    }
}