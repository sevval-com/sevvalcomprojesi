using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Announcement.Queries.GetTodaysAnnouncements
{
    public class GetTodaysAnnouncementsQueryHandler : IRequestHandler<GetTodaysAnnouncementsQueryRequest, ApiResponse<GetTodaysAnnouncementsQueryResponse>>
    {
        private readonly IAnnouncementService _announcementService;

        public GetTodaysAnnouncementsQueryHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<ApiResponse<GetTodaysAnnouncementsQueryResponse>> Handle(GetTodaysAnnouncementsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _announcementService.GetTodaysAnnouncementsAsync(request, cancellationToken);
        }
    }
}
