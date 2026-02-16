using Elite.Core.Models;

namespace Elite.Core.Interfaces.Repositories;

public interface IBrandRepository
{
    Task<Brand?> GetByIdAsync(int id);
    Task<Brand?> GetBySlugAsync(string slug);
    Task<List<Brand>> GetAllAsync(bool includeInactive = false);
    Task<Brand> AddAsync(Brand brand);
    Task<Brand> UpdateAsync(Brand brand);
    Task<bool> DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> SlugExistsAsync(string slug, int? excludeId = null);
    Task<bool> HasProductsAsync(int brandId);
}