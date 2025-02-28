using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig;

internal class ProductEntityConfig : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder
            .ToTable("Products", SchemaNames.PRODUCT)
            .HasIndex(e => e.Title)
            .HasDatabaseName("IX_Products_Title");

        builder
            .HasIndex(e => e.Description)
            .HasDatabaseName("IX_Products_Description");

        // UserName için indeks ekleniyor
        builder
            .HasIndex(e => e.UserName)
            .HasDatabaseName("IX_Products_UserName");
    }
}
