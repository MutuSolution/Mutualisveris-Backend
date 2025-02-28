using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig;

public class LikeEntityConfig : IEntityTypeConfiguration<Like>
{
    public void Configure(EntityTypeBuilder<Like> builder)
    {
        builder
            .ToTable("Likes", SchemaNames.PRODUCT)
            .HasIndex(e => e.Id)
            .HasDatabaseName("IX_Likes_Id");

        builder
            .HasIndex(e => e.UserName)
            .HasDatabaseName("IX_Likes_UserName");
    }
}