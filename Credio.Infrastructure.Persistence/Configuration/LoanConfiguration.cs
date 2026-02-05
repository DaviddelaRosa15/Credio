using Credio.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credio.Infrastructure.Persistence.Configuration;

public class LoanConfiguration : IEntityTypeConfiguration<Loan>
{
    public void Configure(EntityTypeBuilder<Loan> builder)
    {
        builder
            .Property(x => x.Amount)
            .HasPrecision(18, 2);
        
        builder
            .Property(x => x.InterestRate)
            .HasPrecision(18, 2);
    }
}