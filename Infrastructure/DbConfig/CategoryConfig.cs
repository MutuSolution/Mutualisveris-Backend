using Domain;
using Infrastructure.DbConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

public class CategoryConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(c => c.Description)
            .HasMaxLength(1000);

        builder.Property(c => c.ParentCategoryId)
            .IsRequired(false); // ✅ ParentCategory opsiyonel hale getirildi.

        // 🔹 Self-referencing Foreign Key Yapısı
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict); // ❗ Üst kategori silindiğinde alt kategoriler korunur

        builder.ToTable("Categories", SchemaNames.Catalog);
    }
}
