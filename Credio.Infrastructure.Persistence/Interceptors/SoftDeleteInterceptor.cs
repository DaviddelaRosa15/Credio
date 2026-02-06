using Credio.Core.Domain.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Credio.Infrastructure.Persistence.Interceptors;

public class SoftDeleteInterceptor : SaveChangesInterceptor
{
    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = new CancellationToken())
    {
        if (eventData.Context is not null)
        {
            UpdateSoftDeleteEntities(eventData.Context);
        }

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    private void UpdateSoftDeleteEntities(DbContext dbContext)
    {
        IEnumerable<EntityEntry<AuditableBaseEntity>> entries = dbContext.ChangeTracker
            .Entries<AuditableBaseEntity>()
            .Where(x => x.State == EntityState.Deleted);
        
        if (!entries.Any())
        {
            return;
        }

        foreach (EntityEntry<AuditableBaseEntity> entry in entries)
        {
            // Changing state of the delete entries to modified
            entry.State = EntityState.Modified;

            // Updating some important properties of the entity
            entry.Entity.IsDeleted = true;
            entry.Entity.Deleted = DateTime.Now;
        }
    }
}