using Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DbConfig
{
    internal class AddressEntityConfig : IEntityTypeConfiguration<Address>
    {
        public void Configure(EntityTypeBuilder<Address> builder)
        {
            builder.ToTable("Addresses", SchemaNames.User);

            builder.HasKey(a => a.Id);

            builder.Property(a => a.Street)
                .HasMaxLength(200)
                .IsRequired();

            builder.Property(a => a.City)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.Country)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(a => a.ZipCode)
                .HasMaxLength(20)
                .IsRequired();

            builder.Property(a => a.Type)
                .IsRequired();

            builder.HasOne(a => a.User)
                .WithMany(u => u.Addresses)
                .HasForeignKey(a => a.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
