using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Consultant.Queries.GetTotalConsultantCount;

public class GetTotalConsultantCountQueryRequest : IRequest<ApiResponse<GetTotalConsultantCountQueryResponse>>
{
    public const string Route = "/api/v1/total-consultant-count";

    public string? Status { get; set; } // Optional filter for consultant status
    public string? CompanyName { get; set; } // Optional filter for specific company
}
