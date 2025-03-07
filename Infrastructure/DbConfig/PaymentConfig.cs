using Domain;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Infrastructure.DbConfig;

public class PaymentConfig : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder.HasKey(p => p.Id);

        builder.Property(p => p.Amount)
            .HasPrecision(18, 2);

        builder.Property(p => p.Method)
            .HasConversion<int>();

        builder.Property(p => p.Status)
            .HasConversion<int>();

        builder.HasIndex(p => p.PaymentDate);

        builder.ToTable("Payments", SchemaNames.Order);
    }
}
