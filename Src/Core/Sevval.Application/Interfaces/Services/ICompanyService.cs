using Sevval.Application.Features.Common;
using Sevval.Application.Features.Company.Queries.GetCompanyByName;
using Sevval.Application.Features.Company.Queries.GetTotalCompanyCount;

namespace Sevval.Application.Interfaces.Services;

public interface ICompanyService
{
    Task<ApiResponse<List<GetCompaniesQueryResponse>>> GetCompanies(GetCompaniesQueryRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<GetTotalCompanyCountQueryResponse>> GetTotalCompanyCountAsync(GetTotalCompanyCountQueryRequest request, CancellationToken cancellationToken);
}
