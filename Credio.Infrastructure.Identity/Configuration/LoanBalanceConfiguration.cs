using Credio.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credio.Infrastructure.Identity.Configuration;

public class LoanBalanceConfiguration : IEntityTypeConfiguration<LoanBalance>
{
    public void Configure(EntityTypeBuilder<LoanBalance> builder)
    {
        builder
            .Property(x => x.TotalOutstanding)
            .HasPrecision(18, 2);
        
        builder
            .Property(x => x.PrincipalBalance)
            .HasPrecision(18, 2);
        
        builder
            .Property(x => x.InterestBalance)
            .HasPrecision(18, 2);
        
        builder
            .Property(x => x.LateFeeBalance)
            .HasPrecision(18, 2);
    }
}