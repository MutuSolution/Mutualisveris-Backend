using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig
{
    internal class ProductImageEntityConfig : IEntityTypeConfiguration<ProductImage>
    {
        public void Configure(EntityTypeBuilder<ProductImage> builder)
        {
            builder.ToTable("ProductImages", SchemaNames.Catalog);

            builder.Property(pi => pi.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(pi => pi.IsMain)
                .HasDefaultValue(false);
        }
    }
}
