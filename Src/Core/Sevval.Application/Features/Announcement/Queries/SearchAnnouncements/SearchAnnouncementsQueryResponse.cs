namespace Sevval.Application.Features.Announcement.Queries.SearchAnnouncements
{
    public class SearchAnnouncementsQueryResponse
    {
        public List<AnnouncementSearchResultDto> Announcements { get; set; } = new();
        public int TotalCount { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
        public string Message { get; set; } = string.Empty;
        public SearchFiltersDto AppliedFilters { get; set; } = new();
    }

    public class AnnouncementSearchResultDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string Category { get; set; } = string.Empty;
        public string PropertyType { get; set; } = string.Empty;
        public string PropertyStatus { get; set; } = string.Empty;
        public double Area { get; set; }
        public string Province { get; set; } = string.Empty;
        public string District { get; set; } = string.Empty;
        public string Neighborhood { get; set; } = string.Empty;
        public string? RoomCount { get; set; }
        public string? BedroomCount { get; set; }
        public string? BuildingAge { get; set; }
        public string? NetArea { get; set; }
        public string? AdaNo { get; set; }
        public string? ParselNo { get; set; }
        public DateTime CreatedDate { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public bool HasPhotos { get; set; }
        public bool HasVideos { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
    }

    public class SearchFiltersDto
    {
        public string? Keyword { get; set; }
        public string? Category { get; set; }
        public string? PropertyType { get; set; }
        public string? PropertyStatus { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Province { get; set; }
        public string? District { get; set; }
        public string? Neighborhood { get; set; }
        public double? MinArea { get; set; }
        public double? MaxArea { get; set; }
        public string? RoomCount { get; set; }
        public string? BuildingAge { get; set; }
        public bool? HasPhotos { get; set; }
        public bool? HasVideos { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string SortBy { get; set; } = "Date";
        public string SortOrder { get; set; } = "DESC";
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }
}
