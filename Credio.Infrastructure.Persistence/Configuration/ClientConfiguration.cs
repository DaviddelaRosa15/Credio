using Credio.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credio.Infrastructure.Persistence.Configuration;

public class ClientConfiguration : IEntityTypeConfiguration<Client>
{
    public void Configure(EntityTypeBuilder<Client> builder)
    {
        builder
            .HasIndex(x => x.UserId)
            .IsUnique(); 
        
        builder
            .HasIndex(x => x.DocumentNumber)
            .IsUnique();
        
        builder
            .Property(x => x.HomeLatitude)
            .HasPrecision(10, 8);
        
        builder
            .Property(x => x.HomeLongitude)
            .HasPrecision(11, 8);

        builder
            .HasIndex(x => x.LastName)
            .HasDatabaseName("IX_Client_LastName");

        builder
            .HasIndex(x => x.FirstName)
            .HasDatabaseName("IX_Client_FirstName");

        builder
            .HasIndex(x => new { x.FirstName, x.LastName })
            .HasDatabaseName("IX_Client_FullName");
    }
}