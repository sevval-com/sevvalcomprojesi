using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.PropertyType.Queries.GetPropertyTypes;

public class GetPropertyTypesQueryRequest : IRequest<ApiResponse<GetPropertyTypesQueryResponse>>
{
    public const string Route = "/api/v1/property/types";

}
