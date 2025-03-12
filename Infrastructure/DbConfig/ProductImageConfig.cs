using Domain;
using Infrastructure.DbConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class ProductImageConfig : IEntityTypeConfiguration<ProductImage>
{
    public void Configure(EntityTypeBuilder<ProductImage> builder)
    {
        builder.HasKey(pi => pi.Id);

        builder.Property(pi => pi.ImageUrl)
            .IsRequired()
            .HasMaxLength(500);

        builder.HasIndex(pi => new { pi.ProductId, pi.IsMain }).IsUnique(false);

        builder.ToTable("ProductImages", SchemaNames.Catalog);
    }
}
