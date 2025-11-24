using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByUser;
using Sevval.Domain.Entities;

namespace Sevval.Web.Dto.User;

public class UserAnnouncementsDto
{
    public List<GetAnnouncementsByUserQueryResponse>? Announcements { get; set; } = new();
    public ApplicationUser User { get; set; }
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 8;
    public int? TotalPages { get; set; }
    public int? TotalCount { get; set; }
    public bool? HasPreviousPage { get; set; }
    public bool? HasNextPage { get; set; }
    public string Status { get; set; } = "active";
    public string SortBy { get; set; } = "Date";
    public string SortOrder { get; set; } = "DESC";
    public string Email { get; set; }
}
