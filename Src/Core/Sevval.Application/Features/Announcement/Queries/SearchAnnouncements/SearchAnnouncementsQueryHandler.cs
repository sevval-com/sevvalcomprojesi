using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Announcement.Queries.SearchAnnouncements
{
    public class SearchAnnouncementsQueryHandler : IRequestHandler<SearchAnnouncementsQueryRequest, ApiResponse<SearchAnnouncementsQueryResponse>>
    {
        private readonly IAnnouncementService _announcementService;

        public SearchAnnouncementsQueryHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<ApiResponse<SearchAnnouncementsQueryResponse>> Handle(SearchAnnouncementsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _announcementService.SearchAnnouncementsAsync(request, cancellationToken);
        }
    }
}
