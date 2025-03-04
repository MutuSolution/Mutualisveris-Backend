using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig
{
    internal class CategoryEntityConfig : IEntityTypeConfiguration<Category>
    {
        public void Configure(EntityTypeBuilder<Category> builder)
        {
            // Tablo adı ve şema ayarı (örneğin, SchemaNames.CATEGORY sabitini kullandık)
            builder.ToTable("Categories", SchemaNames.CATEGORY);

            // Birincil anahtar konfigürasyonu
            builder.HasKey(c => c.Id);

            // Property Konfigürasyonları
            builder.Property(c => c.Name)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(c => c.Description)
                .HasMaxLength(500);

            builder.Property(c => c.isVisible)
                .HasDefaultValue(true);

            // Hiyerarşik ilişki: ParentCategory - SubCategories
            builder.HasOne(c => c.ParentCategory)
                .WithMany(c => c.SubCategories)
                .HasForeignKey(c => c.ParentCategoryId)
                .OnDelete(DeleteBehavior.Cascade);

            // İndex: İsim alanında sorgu performansı için
            builder.HasIndex(c => c.Name)
                .HasDatabaseName("IX_Categories_Name");
        }
    }
}
