using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.LandType.Queries.GetLandTypes;

public class GetLandTypesQueryRequest : IRequest<ApiResponse<GetLandTypesQueryResponse>>
{
    public const string Route = "/api/v1/lands/types";

}
