using Credio.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Credio.Infrastructure.Persistence.Interceptors;

public class AuditableEntityInterceptor : SaveChangesInterceptor
{
    public override ValueTask<int> SavedChangesAsync(
        SaveChangesCompletedEventData eventData,
        int result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is not null)
        {
            UpdateAuditableEntity(eventData.Context);
        }

        return base.SavedChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateAuditableEntity(DbContext dbContext)
    {
        List<EntityEntry<AuditableBaseEntity>> entities = dbContext.ChangeTracker.Entries<AuditableBaseEntity>()
            .Where(entity => entity.State is EntityState.Added or EntityState.Modified)
            .ToList();
        
        foreach (EntityEntry<AuditableBaseEntity> entity in entities)
        {
            switch (entity.State)
            {
                case EntityState.Added:
                    entity.Entity.Created = DateTime.UtcNow;
                    entity.Entity.CreatedBy = "DefaultBaseUser";
                    break;
                case EntityState.Modified:
                    entity.Entity.LastModified = DateTime.UtcNow;
                    entity.Entity.LastModifiedBy = "DefaultBaseUser";
                    break;
            }
        }
    }
}