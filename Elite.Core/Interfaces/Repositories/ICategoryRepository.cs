using Elite.Core.Models;

namespace Elite.Core.Interfaces.Repositories;

public interface ICategoryRepository
{
    Task<Category?> GetByIdAsync(int id);
    Task<Category?> GetBySlugAsync(string slug);
    Task<List<Category>> GetAllAsync(bool includeInactive = false);
    Task<List<Category>> GetRootCategoriesAsync();
    Task<List<Category>> GetSubCategoriesAsync(int parentId);
    Task<Category> AddAsync(Category category);
    Task<Category> UpdateAsync(Category category);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    Task<bool> HasProductsAsync(int categoryId);
}