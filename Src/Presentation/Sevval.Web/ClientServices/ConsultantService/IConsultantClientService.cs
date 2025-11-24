using Sevval.Application.Features.Common;
using Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;

namespace sevvalemlak.csproj.ClientServices.ConsultantService
{
    public interface IConsultantClientService
    {
        Task<ApiResponse<IList<GetConsultantsByCompanyQueryResponse>>> GetConsultantsByCompany(GetConsultantsByCompanyQueryRequest request, CancellationToken cancellationToken);
    }
}