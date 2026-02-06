using Credio.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credio.Infrastructure.Persistence.Configuration;

public class LateFeeConfiguration : IEntityTypeConfiguration<LateFee>
{
    public void Configure(EntityTypeBuilder<LateFee> builder)
    {
        builder
            .Property(x => x.Amount)
            .HasPrecision(18, 2);
        
        builder
            .Property(x => x.Balance)
            .HasPrecision(18, 2);
    }
}