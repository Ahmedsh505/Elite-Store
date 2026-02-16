using Elite.Core.Enums;

namespace Elite.Core.DTOs.Product;

public class ProductFilterRequest
{
    // Pagination
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 20;

    // Search
    public string? SearchTerm { get; set; }

    // Filters
    public int? CategoryId { get; set; }
    public int? BrandId { get; set; }

    // Price Range
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }

    // Laptop-Specific Filters (Top Priority)
    public List<string>? ProcessorBrands { get; set; } // Intel, AMD
    public List<int>? RamSizes { get; set; } // 8, 16, 32
    public List<int>? StorageCapacities { get; set; } // 256, 512, 1024
    public List<string>? GpuTypes { get; set; } // Integrated, Dedicated
    public List<decimal>? DisplaySizes { get; set; } // 13.3, 15.6, 17.3

    // Condition
    public List<ProductCondition>? Conditions { get; set; }

    // Stock Status
    public List<StockStatus>? StockStatuses { get; set; }

    // Sorting
    public string SortBy { get; set; } = "newest"; // newest, price_low, price_high, name, popular
}