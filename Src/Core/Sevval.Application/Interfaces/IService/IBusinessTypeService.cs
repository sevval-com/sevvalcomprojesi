using Sevval.Application.Features.BusinessType.Queries.GetBusinessTypes;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService
{
    public interface IBusinessTypeService
    {
        Task<ApiResponse<List<GetBusinessTypesQueryResponse>>> GetBusinessTypesAsync(CancellationToken cancellationToken = default);
    }
}
