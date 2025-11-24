using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCount;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByProvince;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByType;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByCompany;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByUser;
using Sevval.Application.Features.Announcement.Queries.GetCompanyAnnouncementCountByProvince;
using Sevval.Application.Features.Announcement.Queries.SearchAnnouncements;
using Sevval.Application.Features.Common;

namespace sevvalemlak.csproj.ClientServices.AnnouncementService;

public interface IAnnouncementClientService
{
    public Task<ApiResponse<GetAnnouncementCountQueryResponse>> GetActiveVisitorCount(GetAnnouncementCountQueryRequest request, CancellationToken cancellationToken);
    public Task<ApiResponse<IList<GetAnnouncementCountByProvinceQueryResponse>>> GetAnnouncementCountByProvince(GetAnnouncementCountByProvinceQueryRequest request, CancellationToken cancellationToken);
    public Task<ApiResponse<IList<GetAnnouncementCountByTypeQueryResponse>>> GetAnnouncementCountByType(GetAnnouncementCountByTypeQueryRequest request, CancellationToken cancellationToken);
    public Task<ApiResponse<List<GetAnnouncementsByCompanyQueryResponse>>> GetAnnouncementsByCompany(GetAnnouncementsByCompanyQueryRequest request, CancellationToken cancellationToken);
    public Task<ApiResponse<List<GetAnnouncementsByUserQueryResponse>>> GetAnnouncementsByUser(GetAnnouncementsByUserQueryRequest request, CancellationToken cancellationToken);
    public Task<ApiResponse<IList<GetCompanyAnnouncementCountByProvinceQueryResponse>>> GetCompanyAnnouncementCountByProvince(GetCompanyAnnouncementCountByProvinceQueryRequest request, CancellationToken cancellationToken);
    Task<ApiResponse<List<SearchAnnouncementsQueryResponse>>> SearchAnnouncements(SearchAnnouncementsQueryRequest request, CancellationToken cancellationToken);
}