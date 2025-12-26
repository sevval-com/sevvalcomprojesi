using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Company.Queries.GetTotalCompanyCount;

public class GetTotalCompanyCountQueryRequest :IRequest<ApiResponse<GetTotalCompanyCountQueryResponse>>
{
    public const string Route = "/api/v1/company/count";

}
