using Domain;
using Infrastructure.DbConfig;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

internal class CategoryEntityConfig : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.ToTable("Categories", SchemaNames.Catalog);

        builder.HasKey(c => c.Id);

        builder.Property(c => c.Name)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(c => c.Description)
            .HasMaxLength(500);

        builder.Property(c => c.IsVisible)
            .HasDefaultValue(true);

        // Hiyerarşik ilişki: ParentCategory - SubCategories
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.NoAction); // ON DELETE NO ACTION

        builder.HasIndex(c => c.Name)
            .HasDatabaseName("IX_Categories_Name");
    }
}