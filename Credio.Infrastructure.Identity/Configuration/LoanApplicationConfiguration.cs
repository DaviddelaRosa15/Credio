using Credio.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credio.Infrastructure.Identity.Configuration;

public class LoanApplicationConfiguration : IEntityTypeConfiguration<LoanApplication>
{
    public void Configure(EntityTypeBuilder<LoanApplication> builder)
    {
        builder
            .Property(x => x.RequestedAmount).
            HasPrecision(18,2);
        
        builder
            .Property(x => x.RequestedInterestRate).
            HasPrecision(5,2);
        
        builder
            .Property(x => x.ApprovedAmount).
            HasPrecision(18,2);
        
        builder
            .Property(x => x.ApprovedInterestRate).
            HasPrecision(5,2);
    }
}