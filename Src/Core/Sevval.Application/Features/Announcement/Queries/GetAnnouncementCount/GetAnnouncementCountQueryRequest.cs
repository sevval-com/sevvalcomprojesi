using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementCount
{
    public class GetAnnouncementCountQueryRequest : IRequest<ApiResponse<GetAnnouncementCountQueryResponse>>
    {
        public const string Route = "/api/v1/announcements/count";
        public string? Status { get; set; } 
    }
}
