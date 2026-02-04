using Credio.Infrastructure.Identity.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Credio.Infrastructure.Identity.Contexts
{
    public class IdentityContext : IdentityDbContext<ApplicationUser>
	{
		public IdentityContext(DbContextOptions<IdentityContext> options) : base(options) { }

		protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
		{
			configurationBuilder.Properties<DateTime>().HaveColumnType("datetime2");
			
			base.ConfigureConventions(configurationBuilder);
		}

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.ApplyConfigurationsFromAssembly(typeof(IdentityContext).Assembly);
			
			base.OnModelCreating(modelBuilder);
		}
		
		public DbSet<Address> Address => Set<Address>();
		
		public DbSet<AmortizationMethod> AmortizationMethod => Set<AmortizationMethod>();
		
		public DbSet<AmortizationSchedule> AmortizationSchedule => Set<AmortizationSchedule>();
		
		public DbSet<AmortizationStatus> AmortizationStatus => Set<AmortizationStatus>();
		
		public DbSet<ApplicationStatus> ApplicationStatus => Set<ApplicationStatus>();
		
		public DbSet<Client> Client => Set<Client>();
		
		public DbSet<Employee> Employee => Set<Employee>();
		
		public DbSet<LateFee> LateFee => Set<LateFee>();
		
		public DbSet<LateFeeStatus> LateFeeStatus => Set<LateFeeStatus>();
		
		public DbSet<Loan> Loan => Set<Loan>();
		
		public DbSet<LoanApplication> LoanApplication => Set<LoanApplication>();
		
		public DbSet<LoanBalance> LoanBalance => Set<LoanBalance>();
		
		public DbSet<LoanStatus> LoanStatus => Set<LoanStatus>();
		
		public DbSet<Payment> Payment => Set<Payment>();
		
		public DbSet<PaymentFrequency> PaymentFrequency => Set<PaymentFrequency>();
		
		public DbSet<PaymentMethod> PaymentMethod => Set<PaymentMethod>();
		
		public DbSet<PaymentStatus> PaymentStatus => Set<PaymentStatus>();
		
		public DbSet<Route> Route => Set<Route>();
		
		public DbSet<SystemSettings> SystemSettings => Set<SystemSettings>();	

        public void TruncateTables()
        {
            var users = Set<ApplicationUser>();
            var roles = Set<IdentityRole>();
            var userRoles = Set<IdentityUserRole<string>>();
            var logins = Set<IdentityUserLogin<string>>();

            users.RemoveRange(users);
            roles.RemoveRange(roles);
            userRoles.RemoveRange(userRoles);
            logins.RemoveRange(logins);

			SaveChanges();
        }
    }
}
