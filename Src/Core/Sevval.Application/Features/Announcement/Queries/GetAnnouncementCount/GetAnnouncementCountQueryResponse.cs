namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementCount
{
    public class GetAnnouncementCountQueryResponse
    {
        public int TotalCount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string? UserEmail { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
