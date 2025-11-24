using Sevval.Application.Features.AboutUs.Queries.GetAboutUs;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.Company.Queries.GetCompanyByName;
using Sevval.Application.Features.Consultant.Queries.GetTotalConsultantCount;

namespace sevvalemlak.csproj.ClientServices.CompanyService
{
    public interface ICompanyClientService
    {
        Task<ApiResponse<GetAboutUsQueryResponse>> GetAboutUs(GetAboutUsQueryRequest request, CancellationToken cancellationToken);
        public Task<ApiResponse<List<GetCompaniesQueryResponse>>> GetCompanies(GetCompaniesQueryRequest request, CancellationToken cancellationToken);
        public Task<ApiResponse<GetTotalConsultantCountQueryResponse>> GetTotalConsultantCount(GetTotalConsultantCountQueryRequest request, CancellationToken cancellationToken);
    }
}
