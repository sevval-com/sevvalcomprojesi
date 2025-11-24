using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementDetails
{
    public class GetAnnouncementDetailsQueryHandler : IRequestHandler<GetAnnouncementDetailsQueryRequest, ApiResponse<GetAnnouncementDetailsQueryResponse>>
    {
        private readonly IAnnouncementService _announcementService;

        public GetAnnouncementDetailsQueryHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<ApiResponse<GetAnnouncementDetailsQueryResponse>> Handle(GetAnnouncementDetailsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _announcementService.GetAnnouncementDetailsAsync(request, cancellationToken);
        }
    }
}
