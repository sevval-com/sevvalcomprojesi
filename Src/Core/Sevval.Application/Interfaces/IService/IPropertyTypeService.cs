using Sevval.Application.Features.PropertyType.Queries.GetPropertyTypes;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService;

public interface IPropertyTypeService
{
    Task<ApiResponse<GetPropertyTypesQueryResponse>> GetPropertyTypesAsync(CancellationToken cancellationToken = default);
}
