using Elite.Core.Interfaces.Repositories;
using Elite.Core.Interfaces.Services;
using Elite.Infrastructure.Data.Context;
using Elite.Infrastructure.Repositories;
using Elite.Services.Services;
using Microsoft.EntityFrameworkCore;

namespace Elite.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddEliteServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                b => b.MigrationsAssembly("Elite.Infrastructure")
            ));

        // Repositories
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IBrandRepository, BrandRepository>();

        // Services
        services.AddScoped<IProductService, ProductService>();

        return services;
    }
}