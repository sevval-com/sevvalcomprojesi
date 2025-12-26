using Sevval.Application.Constants;
using Sevval.Application.Features.AboutUs.Queries.GetAboutUs;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.Company.Queries.GetCompanyByName;
using Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;
using Sevval.Application.Features.Consultant.Queries.GetTotalConsultantCount;

namespace sevvalemlak.csproj.ClientServices.CompanyService;

public class CompanyClientService : ICompanyClientService
{
    private readonly HttpClient _httpClient;
    public CompanyClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<List<GetCompaniesQueryResponse>>> GetCompanies(GetCompaniesQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetCompaniesQueryRequest.Route+$"?Page={request.Page}&Size={request.Size}&companyName={request.CompanyName}&Province={request.Province}&District={request.District}&Search={request.Search}&SortBy={request.SortBy}", cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<GetCompaniesQueryResponse>>>(cancellationToken: cancellationToken);
    }

    public async Task<ApiResponse<GetTotalConsultantCountQueryResponse>> GetTotalConsultantCount(GetTotalConsultantCountQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetTotalConsultantCountQueryRequest.Route, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<GetTotalConsultantCountQueryResponse>>(cancellationToken: cancellationToken);
    }

    public async Task<ApiResponse<GetAboutUsQueryResponse>> GetAboutUs(GetAboutUsQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetAboutUsQueryRequest.Route+ $"?UserId={request.UserId}", cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<GetAboutUsQueryResponse>>(cancellationToken: cancellationToken);
    }
}
