using Sevval.Application.Features.Common;
using Sevval.Application.Features.FacilityType.Queries.GetFacilityTypes;

namespace Sevval.Application.Interfaces.IService
{
    public interface IFacilityTypeService
    {
        Task<ApiResponse<List<GetFacilityTypesQueryResponse>>> GetFacilityTypesAsync(GetFacilityTypesQueryRequest request, CancellationToken cancellationToken = default);
    }
}
