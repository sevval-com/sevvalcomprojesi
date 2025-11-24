using Sevval.Application.Features.Common;
using Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;
using Sevval.Application.Features.Consultant.Queries.GetTotalConsultantCount;

namespace Sevval.Application.Interfaces.Services;

public interface IConsultantService
{
    public Task<ApiResponse<List<GetConsultantsByCompanyQueryResponse>>> GetConsultantsByCompanyAsync(GetConsultantsByCompanyQueryRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<GetTotalConsultantCountQueryResponse>> GetTotalConsultantCountAsync(GetTotalConsultantCountQueryRequest request, CancellationToken cancellationToken);
}
