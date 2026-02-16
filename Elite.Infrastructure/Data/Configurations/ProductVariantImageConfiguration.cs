using Elite.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductVariantImageConfiguration : IEntityTypeConfiguration<ProductVariantImage>
{
    public void Configure(EntityTypeBuilder<ProductVariantImage> builder)
    {
        builder.ToTable("ProductVariantImages");
        builder.HasKey(vi => vi.Id);

        builder.Property(vi => vi.ImageUrl).IsRequired().HasMaxLength(500);
        builder.Property(vi => vi.ThumbnailUrl).HasMaxLength(500);
        builder.Property(vi => vi.AltText).HasMaxLength(200);

        builder.HasOne(vi => vi.ProductVariant)
            .WithMany(v => v.VariantImages)
            .HasForeignKey(vi => vi.ProductVariantId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(vi => vi.ProductVariantId);
    }
}