using sevvalemlak.Dto;

namespace sevvalemlak.csproj.Dto.RealEstates;

public class RealEstateSearchDto
{
    public string? City { get; set; }

    public string? District { get; set; }

    public string? CompanySearch { get; set; }

    public string SortBy { get; set; } = "CompanyName";

    public string SortOrder { get; set; } = "ASC";

    public string AddressFilter { get; set; } = "All";

    public string AnnouncementFilter { get; set; } = "All";

    public int Page { get; set; } = 1;

    public int PageSize { get; set; } = 20;
    public TumIlanlarDTO Announcements { get; set; } = new TumIlanlarDTO();
}
