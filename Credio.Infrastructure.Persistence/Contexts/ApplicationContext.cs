using Credio.Core.Application.Interfaces.Persintence;
using Credio.Core.Domain.Common;
using Credio.Core.Domain.Entities;
using Credio.Infrastructure.Persistence.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace Credio.Infrastructure.Persistence.Contexts
{
    public class ApplicationContext : DbContext, IApplicationContext
    {
        public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options) { }
        
        protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
        {
            configurationBuilder.Properties<DateTime>().HaveColumnType("datetime2");
			
            base.ConfigureConventions(configurationBuilder);
        }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationContext).Assembly);
            
            modelBuilder.ApplyGlobalQueryFilter<AuditableBaseEntity>(x => !x.IsDeleted);

            modelBuilder.ConfigurePostgreSqlTypes();

            //FLUENT API
            base.OnModelCreating(modelBuilder);
        }
        
        public DbSet<Address> Address => Set<Address>();
		
        public DbSet<AmortizationMethod> AmortizationMethod => Set<AmortizationMethod>();
		
        public DbSet<AmortizationSchedule> AmortizationSchedule => Set<AmortizationSchedule>();
		
        public DbSet<AmortizationStatus> AmortizationStatus => Set<AmortizationStatus>();
		
        public DbSet<ApplicationStatus> ApplicationStatus => Set<ApplicationStatus>();
		
        public DbSet<Client> Client => Set<Client>();
        
        public DbSet<DocumentType> DocumentType => Set<DocumentType>(); 
		
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
        
        public async Task<IDbContextTransaction> GetDbTransactionAsync(CancellationToken cancellationToken = default)
        {
	        return await Database.BeginTransactionAsync(cancellationToken);
        }

        public void TruncateTables()
        {
            SaveChanges();
        }
    }
}
