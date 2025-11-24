namespace Sevval.Application.Features.RecentlyVisitedAnnouncement.Commands.AddRecentlyVisitedAnnouncement
{
    public class AddRecentlyVisitedAnnouncementCommandResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public int AnnouncementId { get; set; }
        public DateTime VisitedAt { get; set; }
    }
}
