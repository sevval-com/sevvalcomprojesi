using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.Services;

namespace Sevval.Application.Features.Company.Queries.GetTotalCompanyCount;

public class GetTotalCompanyCountQueryHandler : IRequestHandler<GetTotalCompanyCountQueryRequest, ApiResponse<GetTotalCompanyCountQueryResponse>>
{
    private readonly ICompanyService _CompanyService;

    public GetTotalCompanyCountQueryHandler(ICompanyService CompanyService)
    {
        _CompanyService = CompanyService;
    }

    public async Task<ApiResponse<GetTotalCompanyCountQueryResponse>> Handle(GetTotalCompanyCountQueryRequest request, CancellationToken cancellationToken)
    {
        return await _CompanyService.GetTotalCompanyCountAsync(request, cancellationToken);
    }
}
