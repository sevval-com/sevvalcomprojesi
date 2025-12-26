using MediatR;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Admin.Queries.GetUserStats;

public class GetUserStatsQueryRequest : IRequest<ApiResponse<GetUserStatsQueryResponse>>
{
    public string UserId { get; set; } = string.Empty;
}
