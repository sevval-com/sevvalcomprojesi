using Sevval.Application.Features.FieldStatus.Queries.GetFieldStatuses;
using Sevval.Application.Features.Common;
using System.Threading;
using System.Threading.Tasks;

namespace Sevval.Application.Interfaces.IService
{
    public interface IFieldStatusService
    {
        Task<ApiResponse<GetFieldStatusesQueryResponse>> GetFieldStatusesAsync(CancellationToken cancellationToken);
    }
}
