namespace Elite.Core.DTOs.Product;

public class CreateProductRequest
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }
    public int CategoryId { get; set; }
    public int BrandId { get; set; }
    public decimal BasePrice { get; set; }
    public decimal? CompareAtPrice { get; set; }
    public bool AllowPreOrder { get; set; } = false;
    public int StockAlertThreshold { get; set; } = 2;
    public bool IsFeatured { get; set; } = false;
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }
}