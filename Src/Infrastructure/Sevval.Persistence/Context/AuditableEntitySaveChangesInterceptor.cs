using Sevval.Application.Interfaces.IService.Common;
using Sevval.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;


namespace Sevval.Persistence.Context;

public class AuditableEntitySaveChangesInterceptor(
 ICurrentUserService currentUserService,
 IDateTimeService dateTime) : SaveChangesInterceptor
{
    private readonly ICurrentUserService _currentUserService = currentUserService;
    private readonly IDateTimeService _dateTimeService = dateTime;

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChanges(eventData, result);
    }

    public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        UpdateEntities(eventData.Context);

        return base.SavingChangesAsync(eventData, result, cancellationToken);
    }

    public void UpdateEntities(DbContext? context)
    {
        if (context == null) return;

        foreach (var entry in context.ChangeTracker.Entries<IBaseAuditableEntity>())
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.IsDeleted = false;
                    entry.Entity.CreatedBy = _currentUserService.UserId ?? "";
                    entry.Entity.CreatedDate = _dateTimeService.UtcNow;
                    break;
                case EntityState.Modified:
                    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    entry.Entity.LastModifiedDate = _dateTimeService.UtcNow;
                    break;
                case EntityState.Deleted:
                    entry.State = EntityState.Modified;
                    entry.Entity.IsDeleted = true;
                    entry.Entity.LastModifiedBy = _currentUserService.UserId;
                    entry.Entity.LastModifiedDate = _dateTimeService.UtcNow;
                    break;
            }
        }
    }


}
