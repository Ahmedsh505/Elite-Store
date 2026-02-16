using Elite.Core.Interfaces.Repositories;
using Elite.Core.Models;
using Elite.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;

namespace Elite.Infrastructure.Repositories;

public class BrandRepository : IBrandRepository
{
    private readonly AppDbContext _context;

    public BrandRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Brand?> GetByIdAsync(int id)
    {
        return await _context.Brands.FindAsync(id);
    }

    public async Task<Brand?> GetBySlugAsync(string slug)
    {
        return await _context.Brands.FirstOrDefaultAsync(b => b.Slug == slug);
    }

    public async Task<List<Brand>> GetAllAsync(bool includeInactive = false)
    {
        var query = _context.Brands.AsQueryable();

        if (!includeInactive)
            query = query.Where(b => b.IsActive);

        return await query
            .OrderBy(b => b.SortOrder)
            .ThenBy(b => b.Name)
            .ToListAsync();
    }

    public async Task<Brand> AddAsync(Brand brand)
    {
        await _context.Brands.AddAsync(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task<Brand> UpdateAsync(Brand brand)
    {
        _context.Brands.Update(brand);
        await _context.SaveChangesAsync();
        return brand;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var brand = await _context.Brands.FindAsync(id);
        if (brand == null) return false;

        _context.Brands.Remove(brand);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Brands.AnyAsync(b => b.Id == id);
    }

    public async Task<bool> SlugExistsAsync(string slug, int? excludeId = null)
    {
        var query = _context.Brands.Where(b => b.Slug == slug);
        if (excludeId.HasValue)
            query = query.Where(b => b.Id != excludeId.Value);

        return await query.AnyAsync();
    }

    public async Task<bool> HasProductsAsync(int brandId)
    {
        return await _context.Products.AnyAsync(p => p.BrandId == brandId);
    }
}