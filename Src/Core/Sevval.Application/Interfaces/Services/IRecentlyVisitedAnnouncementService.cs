using Sevval.Application.Features.Announcement.Queries.GetSuitableAnnouncements;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.RecentlyVisitedAnnouncement.Commands.AddRecentlyVisitedAnnouncement;
using Sevval.Application.Features.RecentlyVisitedAnnouncement.Queries.GetRecentlyVisitedAnnouncement;

namespace Sevval.Application.Interfaces.Services
{
    public interface IRecentlyVisitedAnnouncementService
    {
        Task<ApiResponse<AddRecentlyVisitedAnnouncementCommandResponse>> AddRecentlyVisitedAnnouncementAsync(AddRecentlyVisitedAnnouncementCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<List<GetRecentlyVisitedAnnouncementQueryResponse>>> GetRecentlyVisitedAnnouncementAsync(GetRecentlyVisitedAnnouncementQueryRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<List<GetSuitableAnnouncementsQueryResponse>>> GetSuitableAnnouncementsAsync(GetSuitableAnnouncementsQueryRequest request, CancellationToken cancellationToken);
    }
}
