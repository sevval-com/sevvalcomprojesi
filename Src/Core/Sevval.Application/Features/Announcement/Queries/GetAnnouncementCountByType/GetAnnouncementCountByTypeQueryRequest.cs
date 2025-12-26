using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByType
{
    public class GetAnnouncementCountByTypeQueryRequest : IRequest<ApiResponse<IList<GetAnnouncementCountByTypeQueryResponse>>>
    {
        public const string Route = "/api/v1/announcements/count-by-type";
        public string? Status { get; set; }
    }
}
