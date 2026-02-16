using Elite.Core.Enums;

namespace Elite.Core.DTOs.Product;

public class CreateProductVariantRequest
{
    public int ProductId { get; set; }
    public string SKU { get; set; } = string.Empty;

    // Laptop Attributes
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

    // Accessory Attributes
    public string? ConnectionType { get; set; }
    public string? Compatibility { get; set; }

    // Flexible attributes as JSON string
    public string? AdditionalAttributesJson { get; set; }

    // Pricing
    public decimal AddonPrice { get; set; } = 0;
    public decimal? CompareAtAddonPrice { get; set; }

    // Stock & Condition
    public ProductCondition Condition { get; set; } = ProductCondition.New;
    public int StockQuantity { get; set; } = 0;
    public bool IsDefault { get; set; } = false;
    public int SortOrder { get; set; } = 0;
}