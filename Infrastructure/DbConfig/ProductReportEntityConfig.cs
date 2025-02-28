using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig
{
    public class ProductReportEntityConfig : IEntityTypeConfiguration<ProductReport>
    {
        public void Configure(EntityTypeBuilder<ProductReport> builder)
        {
            builder
           .ToTable("ProductReports", SchemaNames.PRODUCT)
           .HasIndex(e => e.Id)
           .HasDatabaseName("IX_ProductReport_Id");

            builder
                .HasIndex(e => e.ProductId)
                .HasDatabaseName("IX_ProductId");
        }
    }
}
