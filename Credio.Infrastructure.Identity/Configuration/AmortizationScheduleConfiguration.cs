using Credio.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credio.Infrastructure.Identity.Configuration;

public class AmortizationScheduleConfiguration : IEntityTypeConfiguration<AmortizationSchedule >
{
    public void Configure(EntityTypeBuilder<AmortizationSchedule> builder)
    {
        builder
            .Property(x => x.DueAmount)
            .HasPrecision(18, 2);
        
        builder
            .Property(x => x.InterestAmount)
            .HasPrecision(18, 2);
        
        builder
            .Property(x => x.PrincipalAmount)
            .HasPrecision(18, 2);
        
        builder
            .Property(x => x.RemainingBalance)
            .HasPrecision(18, 2);
        
        builder
            .Property(x => x.PaidAmount)
            .HasPrecision(18, 2);
    }
}