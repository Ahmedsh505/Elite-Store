using Elite.Core.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductImageConfiguration : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.ToTable("ProductImages");
        builder.HasKey(i => i.Id);

        builder.Property(i => i.ImageUrl).IsRequired().HasMaxLength(500);
        builder.Property(i => i.ThumbnailUrl).HasMaxLength(500);
        builder.Property(i => i.AltText).HasMaxLength(200);

        builder.HasOne(i => i.Product)
            .WithMany(p => p.Images)
            .HasForeignKey(i => i.ProductId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasIndex(i => i.ProductId);
        builder.HasIndex(i => i.IsMain);
    }
}