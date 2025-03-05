using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig
{
    internal class ProductEntityConfig : IEntityTypeConfiguration<Product>
    {
        public void Configure(EntityTypeBuilder<Product> builder)
        {
            builder.ToTable("Products", SchemaNames.Catalog);

            builder.Property(p => p.Name)
                .HasMaxLength(250)
                .IsRequired();

            builder.Property(p => p.Description)
                .HasMaxLength(2000);

            builder.Property(p => p.Price)
                .HasColumnType("decimal(18,2)")
                .IsRequired();

            builder.Property(p => p.SKU)
                .HasMaxLength(50)
                .IsRequired();

            builder.Property(p => p.StockQuantity)
                .HasDefaultValue(0)
                .IsRequired();

            builder.Property(p => p.IsPublic)
                .HasDefaultValue(true);

            builder.Property(p => p.IsDeleted)
                .HasDefaultValue(false);

            builder.HasIndex(p => p.Name)
                .HasDatabaseName("IX_Products_Name");

            builder.HasIndex(p => p.SKU)
                .HasDatabaseName("IX_Products_SKU")
                .IsUnique();

            builder.HasIndex(p => new { p.IsPublic, p.IsDeleted })
                .HasDatabaseName("IX_Products_Visibility");

            builder.HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(p => p.Likes)
                .WithOne(l => l.Product)
                .HasForeignKey(l => l.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(p => p.Images)
                .WithOne(pi => pi.Product)
                .HasForeignKey(pi => pi.ProductId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAdd();

            builder.Property(p => p.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()")
                .ValueGeneratedOnAddOrUpdate()
                .IsRequired(false);
        }
    }
}