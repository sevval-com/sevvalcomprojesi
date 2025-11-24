using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByType
{
    public class GetAnnouncementCountByTypeQueryHandler : IRequestHandler<GetAnnouncementCountByTypeQueryRequest, ApiResponse<IList<GetAnnouncementCountByTypeQueryResponse>>>
    {
        private readonly IAnnouncementService _announcementService;

        public GetAnnouncementCountByTypeQueryHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<ApiResponse<IList<GetAnnouncementCountByTypeQueryResponse>>> Handle(GetAnnouncementCountByTypeQueryRequest request, CancellationToken cancellationToken)
        {
            return await _announcementService.GetAnnouncementCountByTypeAsync(request, cancellationToken);
        }
    }
}
