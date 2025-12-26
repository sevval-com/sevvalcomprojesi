using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.Services;

namespace Sevval.Application.Features.RecentlyVisitedAnnouncement.Queries.GetRecentlyVisitedAnnouncement
{
    public class GetRecentlyVisitedAnnouncementQueryHandler : IRequestHandler<GetRecentlyVisitedAnnouncementQueryRequest, ApiResponse<List<GetRecentlyVisitedAnnouncementQueryResponse>>>
    {
        private readonly IRecentlyVisitedAnnouncementService _recentlyVisitedAnnouncementService;

        public GetRecentlyVisitedAnnouncementQueryHandler(IRecentlyVisitedAnnouncementService recentlyVisitedAnnouncementService)
        {
            _recentlyVisitedAnnouncementService = recentlyVisitedAnnouncementService;
        }

        public async Task<ApiResponse<List<GetRecentlyVisitedAnnouncementQueryResponse>>> Handle(GetRecentlyVisitedAnnouncementQueryRequest request, CancellationToken cancellationToken)
        {



            var result = await _recentlyVisitedAnnouncementService.GetRecentlyVisitedAnnouncementAsync(request, cancellationToken);
            
            return result;

        }
    }
}
