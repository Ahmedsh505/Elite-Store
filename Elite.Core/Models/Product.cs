namespace Elite.Core.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ShortDescription { get; set; }

    // Foreign Keys
    public int CategoryId { get; set; }
    public int BrandId { get; set; }

    // Base Pricing (will be adjusted by variant addons)
    public decimal BasePrice { get; set; }
    public decimal? CompareAtPrice { get; set; } // For "Was X, Now Y" display

    // Product Settings
    public bool IsActive { get; set; } = true;
    public bool IsFeatured { get; set; } = false;
    public bool AllowPreOrder { get; set; } = false;
    public int StockAlertThreshold { get; set; } = 2; // Show "Only X left" when stock <= this

    // SEO & Metadata
    public string? MetaTitle { get; set; }
    public string? MetaDescription { get; set; }
    public string? MetaKeywords { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    public int CreatedBy { get; set; }
    public int? UpdatedBy { get; set; }

    // Navigation Properties
    public Category Category { get; set; } = null!;
    public Brand Brand { get; set; } = null!;
    public ICollection<ProductVariant> Variants { get; set; } = new List<ProductVariant>();
    public ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
}