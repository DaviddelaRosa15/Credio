using Credio.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credio.Infrastructure.Identity.Configuration;

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