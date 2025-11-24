using Sevval.Application.Features.BusinessStatus.Queries.GetBusinessStatuses;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService
{
    public interface IBusinessStatusService
    {
        Task<ApiResponse<List<GetBusinessStatusesQueryResponse>>> GetBusinessStatusesAsync(GetBusinessStatusesQueryRequest request, CancellationToken cancellationToken = default);
    }
}
