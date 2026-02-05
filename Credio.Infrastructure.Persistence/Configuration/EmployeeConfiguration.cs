using Credio.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Credio.Infrastructure.Persistence.Configuration;

public class EmployeeConfiguration : IEntityTypeConfiguration<Employee>
{
    public void Configure(EntityTypeBuilder<Employee> builder)
    {
        builder.HasIndex(x => x.UserId).IsUnique();
        
        builder.HasIndex(x => x.DocumentNumber).IsUnique();
        
        builder.HasIndex(x => x.EmployeeCode).IsUnique();
    }
}