using Sevval.Application.Interfaces.IService.Common;

namespace Sevval.Messaging.Tests.Fixtures;

public sealed class FixedDateTimeService : IDateTimeService
{
    public FixedDateTimeService(DateTime utcNow)
    {
        UtcNow = utcNow;
    }

    public DateTime UtcNow { get; }
}
