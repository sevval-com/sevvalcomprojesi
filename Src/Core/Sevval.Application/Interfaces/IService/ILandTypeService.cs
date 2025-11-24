using Sevval.Application.Features.LandType.Queries.GetLandTypes;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService;

public interface ILandTypeService
{
    Task<ApiResponse<GetLandTypesQueryResponse>> GetLandTypesAsync(CancellationToken cancellationToken = default);
}
