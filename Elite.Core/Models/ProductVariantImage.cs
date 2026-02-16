namespace Elite.Core.Models;

public class ProductVariantImage
{
    public int Id { get; set; }
    public int ProductVariantId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? AltText { get; set; }
    public int SortOrder { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public ProductVariant ProductVariant { get; set; } = null!;
}