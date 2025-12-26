using Sevval.Application.Features.PropertyStatus.Queries.GetPropertyStatuses;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.Services;

public interface IPropertyStatusService
{
    Task<ApiResponse<GetPropertyStatusesQueryResponse>> GetPropertyStatusesAsync(CancellationToken cancellationToken = default);
}
