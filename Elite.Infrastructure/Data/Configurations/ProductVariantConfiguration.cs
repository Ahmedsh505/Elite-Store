using Elite.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Elite.Infrastructure.Data.Configurations;

public class ProductVariantConfiguration : IEntityTypeConfiguration<ProductVariant>
{
    public void Configure(EntityTypeBuilder<ProductVariant> builder)
    {
        builder.ToTable("ProductVariants");

        builder.HasKey(v => v.Id);

        builder.Property(v => v.SKU)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(v => v.SKU)
            .IsUnique();

        // Laptop Attributes
        builder.Property(v => v.ProcessorBrand).HasMaxLength(50);
        builder.Property(v => v.ProcessorModel).HasMaxLength(100);
        builder.Property(v => v.ProcessorGeneration).HasMaxLength(50);
        builder.Property(v => v.ProcessorSpeed).HasColumnType("decimal(5,2)");

        builder.Property(v => v.RamType).HasMaxLength(20);
        builder.Property(v => v.StorageType).HasMaxLength(20);
        builder.Property(v => v.StorageInterface).HasMaxLength(20);

        builder.Property(v => v.GpuType).HasMaxLength(20);
        builder.Property(v => v.GpuBrand).HasMaxLength(50);
        builder.Property(v => v.GpuModel).HasMaxLength(100);

        builder.Property(v => v.DisplaySizeInches).HasColumnType("decimal(4,2)");
        builder.Property(v => v.DisplayResolution).HasMaxLength(50);
        builder.Property(v => v.DisplayPanelType).HasMaxLength(20);

        builder.Property(v => v.OperatingSystem).HasMaxLength(100);
        builder.Property(v => v.Color).HasMaxLength(50);

        // Accessory Attributes
        builder.Property(v => v.ConnectionType).HasMaxLength(50);
        builder.Property(v => v.Compatibility).HasMaxLength(100);

        // Flexible attributes as JSON
        builder.Property(v => v.AdditionalAttributesJson).HasMaxLength(4000);

        // Pricing
        builder.Property(v => v.AddonPrice).HasColumnType("decimal(18,2)").IsRequired();
        builder.Property(v => v.CompareAtAddonPrice).HasColumnType("decimal(18,2)");

        // Relationship
        builder.HasOne(v => v.Product)
            .WithMany(p => p.Variants)
            .HasForeignKey(v => v.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(v => v.VariantImages)
            .WithOne(vi => vi.ProductVariant)
            .HasForeignKey(vi => vi.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for search/filter performance
        builder.HasIndex(v => v.ProductId);
        builder.HasIndex(v => v.ProcessorBrand);
        builder.HasIndex(v => v.RamSizeGB);
        builder.HasIndex(v => v.StorageCapacityGB);
        builder.HasIndex(v => v.GpuType);
        builder.HasIndex(v => v.DisplaySizeInches);
        builder.HasIndex(v => v.Condition);
        builder.HasIndex(v => v.StockQuantity);
        builder.HasIndex(v => v.IsActive);
    }
}