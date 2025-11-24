namespace Sevval.Application.Features.Favorite.Commands.AddFavorite
{
    public class AddFavoriteCommandResponse
    {
        public bool IsSuccessful { get; set; }
        public string Message { get; set; }
        public int AnnouncementId { get; set; }
        public int NewFavoriteCount { get; set; }
        public int WeeklyFavoriteCount { get; set; }
        public string DayOfWeek { get; set; }
    }
}
