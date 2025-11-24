using Sevval.Application.Constants;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCount;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByProvince;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByType;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByCompany;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByUser;
using Sevval.Application.Features.Announcement.Queries.GetCompanyAnnouncementCountByProvince;
using Sevval.Application.Features.Announcement.Queries.SearchAnnouncements;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.Consultant.Queries.GetConsultantsByCompany;

namespace sevvalemlak.csproj.ClientServices.AnnouncementService;

public class AnnouncementClientService : IAnnouncementClientService
{
    private readonly HttpClient _httpClient;
    public AnnouncementClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<GetAnnouncementCountQueryResponse>> GetActiveVisitorCount(GetAnnouncementCountQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetAnnouncementCountQueryRequest.Route, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<GetAnnouncementCountQueryResponse>>(cancellationToken: cancellationToken);
    }

    public async Task<ApiResponse<IList<GetAnnouncementCountByProvinceQueryResponse>>> GetAnnouncementCountByProvince(GetAnnouncementCountByProvinceQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetAnnouncementCountByProvinceQueryRequest.Route, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<IList<GetAnnouncementCountByProvinceQueryResponse>>>(cancellationToken: cancellationToken);
    }

    public async Task<ApiResponse<IList<GetAnnouncementCountByTypeQueryResponse>>> GetAnnouncementCountByType(GetAnnouncementCountByTypeQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetAnnouncementCountByTypeQueryRequest.Route+$"?Status={request.Status}", cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<IList<GetAnnouncementCountByTypeQueryResponse>>>(cancellationToken: cancellationToken);
    }

    public async Task<ApiResponse<IList<GetCompanyAnnouncementCountByProvinceQueryResponse>>> GetCompanyAnnouncementCountByProvince(GetCompanyAnnouncementCountByProvinceQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetCompanyAnnouncementCountByProvinceQueryRequest.Route + $"?Status={request.Status}&UserId={request.UserId}", cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<IList<GetCompanyAnnouncementCountByProvinceQueryResponse>>>(cancellationToken: cancellationToken);
    }

    public async Task<ApiResponse<List<GetAnnouncementsByCompanyQueryResponse>>> GetAnnouncementsByCompany(GetAnnouncementsByCompanyQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetAnnouncementsByCompanyQueryRequest.Route + $"?Status={request.Status}&UserId={request.UserId}&Page={request.Page}&Size={request.Size}", cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<GetAnnouncementsByCompanyQueryResponse>>>(cancellationToken: cancellationToken);
    }

    public async Task<ApiResponse<List<GetAnnouncementsByUserQueryResponse>>> GetAnnouncementsByUser(GetAnnouncementsByUserQueryRequest request, CancellationToken cancellationToken)
    {
        var queryParams = $"?Email={Uri.EscapeDataString(request.Email)}&Status={request.Status}&Page={request.Page}&Size={request.Size}";
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetAnnouncementsByUserQueryRequest.Route + queryParams, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<GetAnnouncementsByUserQueryResponse>>>(cancellationToken: cancellationToken);
    }

    public async Task<ApiResponse<List<SearchAnnouncementsQueryResponse>>> SearchAnnouncements(SearchAnnouncementsQueryRequest request, CancellationToken cancellationToken)
    {
        var queryParams = $"?Status={request.Status}&Page={request.Page}&PageSize={request.PageSize}";
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + SearchAnnouncementsQueryRequest.Route + queryParams, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<SearchAnnouncementsQueryResponse>>>(cancellationToken: cancellationToken);
    }

    
}
