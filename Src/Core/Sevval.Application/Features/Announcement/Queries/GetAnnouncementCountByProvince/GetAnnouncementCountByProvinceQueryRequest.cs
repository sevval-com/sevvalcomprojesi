using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByProvince
{
    public class GetAnnouncementCountByProvinceQueryRequest : IRequest<ApiResponse<IList<GetAnnouncementCountByProvinceQueryResponse>>>
    {
        public const string Route = "/api/v1/announcements/count-by-province";
        public string? Status { get; set; }
    }
}
