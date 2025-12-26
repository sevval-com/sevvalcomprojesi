using Sevval.Persistence.Context;

namespace Sevval.Infrastructure.Services;

public abstract class BaseService
{
    public ApplicationDbContext DbContext { get; }

    protected BaseService(ApplicationDbContext dbContext)
    {
        DbContext = dbContext;
    }
}

