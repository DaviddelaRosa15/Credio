using Credio.Core.Application.Interfaces.Services;
using Credio.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.DependencyInjection;

namespace Credio.Infrastructure.Persistence.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    private readonly IServiceScopeFactory _scopeFactory;

    public AuditableEntityInterceptor(IServiceScopeFactory scopeFactory)
    {
        _scopeFactory = scopeFactory;
    }
    
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntity(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntity(DbContext dbContext)
    {
        using IServiceScope scope = _scopeFactory.CreateScope();
        
        ICurrentUserService currentUserService =  scope.ServiceProvider.GetRequiredService<ICurrentUserService>();
        
        string? currenUserName = currentUserService.GetCurrentUserName();
        
        List<EntityEntry<AuditableBaseEntity>> entities = dbContext.ChangeTracker.Entries<AuditableBaseEntity>()
            .Where(entity => entity.State is EntityState.Added or EntityState.Modified)
            .ToList();
        
        foreach (EntityEntry<AuditableBaseEntity> entity in entities)
        {
            switch (entity.State)
            {
                case EntityState.Added:
                    entity.Entity.Created = DateTime.Now;
                    entity.Entity.CreatedBy = currenUserName ?? "System";
                    break;
                case EntityState.Modified:
                    entity.Entity.LastModified = DateTime.Now;
                    entity.Entity.LastModifiedBy = currenUserName ?? "System";
                    break;
            }
        }
    }
}