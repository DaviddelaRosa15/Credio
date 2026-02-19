using Credio.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credio.Infrastructure.Persistence.Configuration;

public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
{
    public void Configure(EntityTypeBuilder<Payment> builder)
    {
        builder
            .Property(x => x.AmountPaid)
            .HasPrecision(18, 2);
        
        builder
            .Property(x => x.GpsLatitude)
            .HasPrecision(10, 8);
        
        builder
            .Property(x => x.GpsLongitude)
            .HasPrecision(11, 8);

    }
}