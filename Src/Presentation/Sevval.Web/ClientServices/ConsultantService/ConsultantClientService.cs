using Sevval.Application.Constants;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;

namespace sevvalemlak.csproj.ClientServices.ConsultantService;

public class ConsultantClientService: IConsultantClientService
{
    private readonly HttpClient _httpClient;
    public ConsultantClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<IList<GetConsultantsByCompanyQueryResponse>>> GetConsultantsByCompany(GetConsultantsByCompanyQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetConsultantsByCompanyQueryRequest.Route+$"?UserId={request.UserId}&Page={request.Page}&Size={request.Size}", cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<IList<GetConsultantsByCompanyQueryResponse>>>(cancellationToken: cancellationToken);
    }
}
