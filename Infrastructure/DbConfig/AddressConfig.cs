
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.DbConfig;
using Domain;

public class AddressConfig : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.HasKey(a => a.Id);

        builder.Property(a => a.Street)
            .IsRequired()
            .HasMaxLength(500);

        builder.Property(a => a.City)
            .IsRequired()
            .HasMaxLength(255);

        builder.Property(a => a.ZipCode)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(a => a.PhoneNumber)
            .IsRequired()
            .HasMaxLength(15)
            .HasColumnType("VARCHAR(15)");

        builder.Property(a => a.Type)
            .HasConversion<int>();

        builder.ToTable("Addresses", SchemaNames.User);
    }
}
