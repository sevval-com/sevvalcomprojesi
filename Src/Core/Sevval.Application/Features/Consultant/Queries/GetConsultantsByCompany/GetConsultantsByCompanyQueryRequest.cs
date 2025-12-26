using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;

public class GetConsultantsByCompanyQueryRequest : IRequest<ApiResponse<List<GetConsultantsByCompanyQueryResponse>>>
{
    public const string Route = "/api/v1/consultant/company";

    public string UserId { get; set; }
    public string? Status { get; set; }
    public int Page { get; set; } = 1;
    public int Size { get; set; } = 200;

}
