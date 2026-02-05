using Credio.Core.Domain.Common;
using Credio.Core.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Persistence.Contexts
{
    public class ApplicationContext : DbContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        public DbSet<Client> Clients { get; set; }
        public DbSet<Employee> Employees { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<AuditableBaseEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.Created = DateTime.UtcNow;
                        entry.Entity.CreatedBy = "DefaultBaseUser";
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModified = DateTime.UtcNow;
                        entry.Entity.LastModifiedBy = "DefaultBaseUser";
                        break;
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //FLUENT API
            base.OnModelCreating(modelBuilder);

            #region Tables
            modelBuilder.Entity<Client>()
                .ToTable("Clients");

            modelBuilder.Entity<Employee>()
                .ToTable("Employees");
            #endregion

            #region Primary keys
            modelBuilder.Entity<Client>()
                .HasKey(x => x.Id);

            modelBuilder.Entity<Employee>()
                .HasKey(x => x.Id);
            #endregion

            #region Relationships
            modelBuilder.Entity<Client>()
                .HasOne<Employee>(x => x.Officer)
                .WithMany(x => x.Clients)
                .HasForeignKey(x => x.OfficerId);
            #endregion

            #region Property configurations
            modelBuilder.Entity<Client>().HasIndex(x => x.DocumentNumber).IsUnique();
            modelBuilder.Entity<Client>().HasIndex(x => x.UserId).IsUnique();

            modelBuilder.Entity<Employee>().HasIndex(x => x.DocumentNumber).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(x => x.EmployeeCode).IsUnique();
            modelBuilder.Entity<Employee>().HasIndex(x => x.UserId).IsUnique();
            #endregion
        }

        public void TruncateTables()
        {
            SaveChanges();
        }
    }
}
