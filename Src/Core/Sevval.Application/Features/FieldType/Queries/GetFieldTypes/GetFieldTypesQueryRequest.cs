using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.FieldType.Queries.GetFieldTypes;

public class GetFieldTypesQueryRequest : IRequest<ApiResponse<GetFieldTypesQueryResponse>>
{
    public const string Route = "/api/v1/fields/types";

}
