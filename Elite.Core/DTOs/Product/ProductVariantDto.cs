using Elite.Core.Enums;

namespace Elite.Core.DTOs.Product;

public class ProductVariantDto
{
    public int Id { get; set; }
    public string SKU { get; set; } = string.Empty;

    // Laptop Specs
    public string? ProcessorBrand { get; set; }
    public string? ProcessorModel { get; set; }
    public string? ProcessorGeneration { get; set; }
    public decimal? ProcessorSpeed { get; set; }

    public int? RamSizeGB { get; set; }
    public string? RamType { get; set; }
    public int? RamSpeed { get; set; }

    public string? StorageType { get; set; }
    public int? StorageCapacityGB { get; set; }
    public string? StorageInterface { get; set; }

    public string? GpuType { get; set; }
    public string? GpuBrand { get; set; }
    public string? GpuModel { get; set; }
    public int? GpuVramGB { get; set; }

    public decimal? DisplaySizeInches { get; set; }
    public string? DisplayResolution { get; set; }
    public int? DisplayRefreshRate { get; set; }
    public string? DisplayPanelType { get; set; }

    public string? OperatingSystem { get; set; }
    public string? Color { get; set; }
    public int? WarrantyMonths { get; set; }

    // Accessory Specs
    public string? ConnectionType { get; set; }
    public string? Compatibility { get; set; }

    public string? AdditionalAttributesJson { get; set; }

    // Pricing (calculated: base + addon)
    public decimal FinalPrice { get; set; }
    public decimal? FinalCompareAtPrice { get; set; }
    public decimal AddonPrice { get; set; }

    // Stock & Condition
    public ProductCondition Condition { get; set; }
    public string ConditionDisplay { get; set; } = string.Empty;
    public int StockQuantity { get; set; }
    public StockStatus StockStatus { get; set; }
    public string StockStatusDisplay { get; set; } = string.Empty;

    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }

    public List<ProductImageDto> Images { get; set; } = new();
}