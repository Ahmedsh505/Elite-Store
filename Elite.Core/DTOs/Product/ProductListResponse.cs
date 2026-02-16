using Elite.Core.Enums;

namespace Elite.Core.DTOs.Product;

public class ProductListResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? ShortDescription { get; set; }
    public decimal StartingPrice { get; set; } // Lowest variant price
    public decimal? CompareAtStartingPrice { get; set; }
    public bool IsFeatured { get; set; }

    public string CategoryName { get; set; } = string.Empty;
    public string BrandName { get; set; } = string.Empty;
    public string? MainImageUrl { get; set; }

    public int TotalVariants { get; set; }
    public int InStockVariants { get; set; }
    public StockStatus OverallStockStatus { get; set; }
    public List<ProductCondition> AvailableConditions { get; set; } = new();

    public DateTime CreatedAt { get; set; }
}