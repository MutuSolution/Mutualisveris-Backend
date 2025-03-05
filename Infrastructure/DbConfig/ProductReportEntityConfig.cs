using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig
{
    internal class ProductReportEntityConfig : IEntityTypeConfiguration<ProductReport>
    {
        public void Configure(EntityTypeBuilder<ProductReport> builder)
        {
            builder.ToTable("ProductReports", SchemaNames.Catalog);

            builder.HasIndex(e => e.Id)
                .HasDatabaseName("IX_ProductReports_Id");

            builder.HasIndex(e => e.ProductId)
                .HasDatabaseName("IX_ProductReports_ProductId");
        }
    }
}
