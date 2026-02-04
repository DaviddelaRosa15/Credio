using Credio.Infrastructure.Identity.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credio.Infrastructure.Identity.Configuration;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder
            .Property(x => x.HomeLatitude)
            .HasPrecision(10, 8);
        
        builder
            .Property(x => x.HomeLongitude)
            .HasPrecision(11, 8);
    }
}