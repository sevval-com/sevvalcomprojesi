using Sevval.Domain.Entities;
using Sevval.Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;


namespace Sevval.Application.Interfaces.IService.Common;

public interface IApplicationDbContext
{
    #region Entities


    DbSet<Audit> AuditLogs { get; }      
    DbSet<Country> Countries { get; }
    DbSet<District> Districts { get; }
    DbSet<ForgettenPassword> ForgettenPasswords { get;  }
    public DbSet<ApplicationUser> Users { get;  }
   

    #endregion

    Task<int> SaveChangesAsync(CancellationToken cancellationToken);
    Task<int> SaveChangesWithIdentityInsertAsync<T>(CancellationToken cancellationToken);
}
