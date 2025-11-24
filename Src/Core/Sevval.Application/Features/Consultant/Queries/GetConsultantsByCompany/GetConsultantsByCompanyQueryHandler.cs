using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.Services;

namespace Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;

public class GetConsultantsByCompanyQueryHandler : IRequestHandler<GetConsultantsByCompanyQueryRequest, ApiResponse<List<GetConsultantsByCompanyQueryResponse>>>
{
    private readonly IConsultantService _consultantService;

    public GetConsultantsByCompanyQueryHandler(IConsultantService consultantService)
    {
        _consultantService = consultantService;
    }

    public async Task<ApiResponse<List<GetConsultantsByCompanyQueryResponse>>> Handle(GetConsultantsByCompanyQueryRequest request, CancellationToken cancellationToken)
    {
        return await _consultantService.GetConsultantsByCompanyAsync(request, cancellationToken);
    }
}
