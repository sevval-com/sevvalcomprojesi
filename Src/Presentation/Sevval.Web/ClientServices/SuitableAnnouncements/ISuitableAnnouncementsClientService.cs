using Sevval.Application.Features.Common;
using Sevval.Application.Features.Announcement.Queries.GetSuitableAnnouncements;

namespace sevvalemlak.csproj.ClientServices.SuitableAnnouncements;

public interface ISuitableAnnouncementsClientService
{
    Task<ApiResponse<List<GetSuitableAnnouncementsQueryResponse>>> GetSuitableAnnouncementsAsync(GetSuitableAnnouncementsQueryRequest request, CancellationToken cancellationToken);
}
