using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByUser
{
    public class GetAnnouncementsByUserQueryRequest : IRequest<ApiResponse<List<GetAnnouncementsByUserQueryResponse>>>
    {
        public const string Route = "/api/v1/announcements/user";

        public string Email { get; set; }
        public string? Status { get; set; }
        public string? SortOrder { get; set; } = "DESC";
        public string? SortBy { get; set; }
        public int Page { get; set; } = 1;
        public int Size { get; set; } = 10;
    
    }
}
