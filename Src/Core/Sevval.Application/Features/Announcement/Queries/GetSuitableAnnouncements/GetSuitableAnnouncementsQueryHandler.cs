using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.Services;

namespace Sevval.Application.Features.Announcement.Queries.GetSuitableAnnouncements
{
    public class GetSuitableAnnouncementsQueryHandler : IRequestHandler<GetSuitableAnnouncementsQueryRequest, ApiResponse<List<GetSuitableAnnouncementsQueryResponse>>>
    {
        private readonly IRecentlyVisitedAnnouncementService _recentlyVisitedAnnouncementService;

        public GetSuitableAnnouncementsQueryHandler(IRecentlyVisitedAnnouncementService recentlyVisitedAnnouncementService)
        {
            _recentlyVisitedAnnouncementService = recentlyVisitedAnnouncementService;
        }



        public async Task<ApiResponse<List<GetSuitableAnnouncementsQueryResponse>>> Handle(GetSuitableAnnouncementsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _recentlyVisitedAnnouncementService.GetSuitableAnnouncementsAsync(request, cancellationToken);
        }
    }
}
