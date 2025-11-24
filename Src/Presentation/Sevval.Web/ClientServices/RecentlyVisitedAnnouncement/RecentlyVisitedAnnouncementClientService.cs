using Sevval.Application.Constants;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.RecentlyVisitedAnnouncement.Commands.AddRecentlyVisitedAnnouncement;
using Sevval.Application.Features.RecentlyVisitedAnnouncement.Queries.GetRecentlyVisitedAnnouncement;

namespace sevvalemlak.csproj.ClientServices.RecentlyVisitedAnnouncement;

public class RecentlyVisitedAnnouncementClientService : IRecentlyVisitedAnnouncementClientService
{
    private readonly HttpClient _httpClient;
    public RecentlyVisitedAnnouncementClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }


    public async Task<ApiResponse<AddRecentlyVisitedAnnouncementCommandResponse>> AddRecentlyVisitedAnnouncementAsync(AddRecentlyVisitedAnnouncementCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.PostAsJsonAsync(GeneralConstants.BaseClientUrl + AddRecentlyVisitedAnnouncementCommandRequest.Route, request, cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<AddRecentlyVisitedAnnouncementCommandResponse>>(cancellationToken: cancellationToken);
    }

    public async Task<ApiResponse<List<GetRecentlyVisitedAnnouncementQueryResponse>>> GetRecentlyVisitedAnnouncementAsync(GetRecentlyVisitedAnnouncementQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetRecentlyVisitedAnnouncementQueryRequest.Route+$"?UserId={request.UserId}&page={request.Page}&size={request.Size}", cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<GetRecentlyVisitedAnnouncementQueryResponse>>>(cancellationToken: cancellationToken);
    }
}
