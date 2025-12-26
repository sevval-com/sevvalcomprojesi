using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Announcement.Queries.GetCompanyAnnouncementCountByProvince
{
    public class GetCompanyAnnouncementCountByProvinceQueryHandler : IRequestHandler<GetCompanyAnnouncementCountByProvinceQueryRequest, ApiResponse<IList<GetCompanyAnnouncementCountByProvinceQueryResponse>>>
    {
        private readonly IAnnouncementService _announcementService;

        public GetCompanyAnnouncementCountByProvinceQueryHandler(IAnnouncementService announcementService)
        {
            _announcementService = announcementService;
        }

        public async Task<ApiResponse<IList<GetCompanyAnnouncementCountByProvinceQueryResponse>>> Handle(GetCompanyAnnouncementCountByProvinceQueryRequest request, CancellationToken cancellationToken)
        {
            return await _announcementService.GetCompanyAnnouncementCountByProvinceAsync(request, cancellationToken);
        }
    }
}
