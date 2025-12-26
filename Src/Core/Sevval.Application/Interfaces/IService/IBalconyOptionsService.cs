using Sevval.Application.Features.BalconyOptions.Queries.GetBalconyOptions;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService
{
    public interface IBalconyOptionsService
    {
        Task<ApiResponse<List<GetBalconyOptionsQueryResponse>>> GetBalconyOptionsAsync(GetBalconyOptionsQueryRequest request, CancellationToken cancellationToken);
    }
}
