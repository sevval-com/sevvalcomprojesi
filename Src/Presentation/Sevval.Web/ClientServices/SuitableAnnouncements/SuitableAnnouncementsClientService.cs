using Sevval.Application.Constants;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.Announcement.Queries.GetSuitableAnnouncements;

namespace sevvalemlak.csproj.ClientServices.SuitableAnnouncements;

public class SuitableAnnouncementsClientService : ISuitableAnnouncementsClientService
{
    private readonly HttpClient _httpClient;
    
    public SuitableAnnouncementsClientService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApiResponse<List<GetSuitableAnnouncementsQueryResponse>>> GetSuitableAnnouncementsAsync(GetSuitableAnnouncementsQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetSuitableAnnouncementsQueryRequest.Route + $"?UserId={request.UserId}&page={request.Page}&size={request.Size}", cancellationToken);
        return await response.Content.ReadFromJsonAsync<ApiResponse<List<GetSuitableAnnouncementsQueryResponse>>>(cancellationToken: cancellationToken);
    }
}
