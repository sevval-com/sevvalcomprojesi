using Sevval.Application.Features.AboutUs.Queries.GetAboutUs;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByCompany;

namespace sevvalemlak.csproj.Dto.Company;

public class CompanyDetailDto
{
    public string UserId { get; set; }
    public List<GetAnnouncementsByCompanyQueryResponse> Announcements { get; set; }
    public GetAboutUsQueryResponse About { get; set; }
    
    // Pagination properties
    public int CurrentPage { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int TotalPages { get; set; }
    public int TotalCount { get; set; }
    public bool HasPreviousPage { get; set; }
    public bool HasNextPage { get; set; }
}
