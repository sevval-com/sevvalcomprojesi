using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementCount
{
    public class GetAnnouncementCountQueryHandler : BaseHandler, IRequestHandler<GetAnnouncementCountQueryRequest, ApiResponse<GetAnnouncementCountQueryResponse>>
    {
        private readonly IAnnouncementService _announcementService;

        public GetAnnouncementCountQueryHandler(IHttpContextAccessor httpContextAccessor, IAnnouncementService announcementService) : base(httpContextAccessor)
        {
            _announcementService = announcementService;
        }

        public async Task<ApiResponse<GetAnnouncementCountQueryResponse>> Handle(GetAnnouncementCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _announcementService.GetAnnouncementCountAsync(request, cancellationToken);

            return response;
        }
    }
}
