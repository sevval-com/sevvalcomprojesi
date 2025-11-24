using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.Services;

namespace Sevval.Application.Features.Company.Queries.GetCompanyByName
{
    public class GetCompaniesQueryHandler : IRequestHandler<GetCompaniesQueryRequest, ApiResponse<List<GetCompaniesQueryResponse>>>
    {
        private readonly ICompanyService _companyService;

        public GetCompaniesQueryHandler(ICompanyService companyService)
        {
            _companyService = companyService;
        }

        public async Task<ApiResponse<List<GetCompaniesQueryResponse>>> Handle(GetCompaniesQueryRequest request, CancellationToken cancellationToken)
        {
            return await _companyService.GetCompanies(request, cancellationToken);
        }
    }
}
