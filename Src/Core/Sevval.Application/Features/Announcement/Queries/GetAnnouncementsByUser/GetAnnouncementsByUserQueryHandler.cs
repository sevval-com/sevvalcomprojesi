using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByUser
{
    public class GetAnnouncementsByUserQueryHandler : IRequestHandler<GetAnnouncementsByUserQueryRequest, ApiResponse<List<GetAnnouncementsByUserQueryResponse>>>
    {
        private readonly IAnnouncementService _announcementService;

        public GetAnnouncementsByUserQueryHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<ApiResponse<List<GetAnnouncementsByUserQueryResponse>>> Handle(GetAnnouncementsByUserQueryRequest request, CancellationToken cancellationToken)
        {
            return await _announcementService.GetAnnouncementsByUserAsync(request, cancellationToken);
        }
    }
}
