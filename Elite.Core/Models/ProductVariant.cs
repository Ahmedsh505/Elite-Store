using Elite.Core.Enums;

namespace Elite.Core.Models;

public class ProductVariant
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty; // Unique identifier

    // Variant-Specific Attributes (Structured for fast search/filter)
    // LAPTOPS
    public string? ProcessorBrand { get; set; } // Intel, AMD
    public string? ProcessorModel { get; set; } // i7-12700H, Ryzen 7 5800H
    public string? ProcessorGeneration { get; set; } // 12th Gen, 5000 Series
    public decimal? ProcessorSpeed { get; set; } // 2.4 GHz

    public int? RamSizeGB { get; set; } // 8, 16, 32
    public string? RamType { get; set; } // DDR4, DDR5
    public int? RamSpeed { get; set; } // 3200 MHz

    public string? StorageType { get; set; } // SSD, HDD
    public int? StorageCapacityGB { get; set; } // 256, 512, 1024
    public string? StorageInterface { get; set; } // NVMe, SATA

    public string? GpuType { get; set; } // Integrated, Dedicated
    public string? GpuBrand { get; set; } // NVIDIA, AMD, Intel
    public string? GpuModel { get; set; } // RTX 3060, RX 6600M
    public int? GpuVramGB { get; set; }

    public decimal? DisplaySizeInches { get; set; } // 13.3, 15.6, 17.3
    public string? DisplayResolution { get; set; } // 1920x1080, 2560x1440
    public int? DisplayRefreshRate { get; set; } // 60, 144, 240
    public string? DisplayPanelType { get; set; } // IPS, OLED, TN

    public string? OperatingSystem { get; set; } // Windows 11, None
    public string? Color { get; set; }
    public int? WarrantyMonths { get; set; }

    // ACCESSORIES (Keyboards, Mice, Headsets, etc.)
    public string? ConnectionType { get; set; } // Wired, Wireless, Bluetooth
    public string? Compatibility { get; set; } // Windows, Mac, Universal

    // Flexible Attributes (for extras like RGB, Mechanical, etc.)
    // Stored as JSON: {"RGB": true, "Mechanical": true, "WirelessRange": "10m"}
    public string? AdditionalAttributesJson { get; set; }

    // Pricing (Base + Addon)
    public decimal AddonPrice { get; set; } = 0; // Added to Product.BasePrice
    public decimal? CompareAtAddonPrice { get; set; }

    // Condition & Stock
    public ProductCondition Condition { get; set; } = ProductCondition.New;
    public int StockQuantity { get; set; } = 0;

    // Variant Settings
    public bool IsDefault { get; set; } = false; // Show this variant by default
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }

    // Timestamps
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }

    // Navigation Properties
    public Product Product { get; set; } = null!;
    public ICollection<ProductVariantImage> VariantImages { get; set; } = new List<ProductVariantImage>();
}