namespace Sevval.Application.Features.Video.Queries.GetVideos
{
    public class GetVideosQueryResponse
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string VideoUrl { get; set; } = string.Empty;
        public string ThumbnailUrl { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public int ViewCount { get; set; }
        public int Duration { get; set; } // in seconds
    }

    
}
