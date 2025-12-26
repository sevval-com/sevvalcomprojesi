using Sevval.Application.Features.Common;
using Sevval.Application.Features.GardenType.Queries.GetGardenTypes;

namespace Sevval.Application.Interfaces.IService;

public interface IGardenTypeService
{
    Task<ApiResponse<List<GetGardenTypesQueryResponse>>> GetGardenTypesAsync(CancellationToken cancellationToken = default);
}
