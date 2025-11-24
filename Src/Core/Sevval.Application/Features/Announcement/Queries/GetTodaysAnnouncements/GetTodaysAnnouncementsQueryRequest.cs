using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Announcement.Queries.GetTodaysAnnouncements
{
    public class GetTodaysAnnouncementsQueryRequest : IRequest<ApiResponse<GetTodaysAnnouncementsQueryResponse>>
    {
        public const string Route = "/api/v1/announcements/todays";
        public string? Status { get; set; } = "active"; // Default to active announcements
    }
}
