using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Announcement.Queries.GetCompanyAnnouncementCountByProvince
{
    public class GetCompanyAnnouncementCountByProvinceQueryRequest : IRequest<ApiResponse<IList<GetCompanyAnnouncementCountByProvinceQueryResponse>>>
    {
        public const string Route = "/api/v1/announcements/company/count-by-province";
        
        public string UserId { get; set; }
        public string? Status { get; set; }
    }
}
