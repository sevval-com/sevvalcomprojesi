using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.Services;

namespace Sevval.Application.Features.Consultant.Queries.GetTotalConsultantCount;

public class GetTotalConsultantCountQueryHandler : IRequestHandler<GetTotalConsultantCountQueryRequest, ApiResponse<GetTotalConsultantCountQueryResponse>>
{
    private readonly IConsultantService _consultantService;

    public GetTotalConsultantCountQueryHandler(IConsultantService consultantService)
    {
        _consultantService = consultantService;
    }

    public async Task<ApiResponse<GetTotalConsultantCountQueryResponse>> Handle(GetTotalConsultantCountQueryRequest request, CancellationToken cancellationToken)
    {
        return await _consultantService.GetTotalConsultantCountAsync(request, cancellationToken);
    }
}
