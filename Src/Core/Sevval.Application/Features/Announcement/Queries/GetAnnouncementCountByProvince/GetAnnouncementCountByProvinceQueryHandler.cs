using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByProvince
{
    public class GetAnnouncementCountByProvinceQueryHandler : IRequestHandler<GetAnnouncementCountByProvinceQueryRequest, ApiResponse<IList<GetAnnouncementCountByProvinceQueryResponse>>>
    {
        private readonly IAnnouncementService _announcementService;

        public GetAnnouncementCountByProvinceQueryHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<ApiResponse<IList<GetAnnouncementCountByProvinceQueryResponse>>> Handle(GetAnnouncementCountByProvinceQueryRequest request, CancellationToken cancellationToken)
        {
            return await _announcementService.GetAnnouncementCountByProvinceAsync(request, cancellationToken);
        }
    }
}
