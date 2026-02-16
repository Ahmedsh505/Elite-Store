namespace Elite.Core.Models;

public class ProductImage
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ImageUrl { get; set; } = string.Empty;
    public string? ThumbnailUrl { get; set; }
    public string? AltText { get; set; }
    public bool IsMain { get; set; } = false;
    public int SortOrder { get; set; }
    public DateTime UploadedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public Product Product { get; set; } = null!;
}