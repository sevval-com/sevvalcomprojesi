using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCount;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByType;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByProvince;
using Sevval.Application.Features.Announcement.Queries.GetCompanyAnnouncementCountByProvince;
using Sevval.Application.Features.Announcement.Queries.GetTodaysAnnouncements;
using Sevval.Application.Features.Announcement.Queries.SearchAnnouncements;
using Sevval.Application.Features.Announcement.Queries.GetSuitableAnnouncements;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByCompany;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByUser;
using Sevval.Application.Features.Announcement.Queries.GetAnnouncementDetails;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService
{
    public interface IAnnouncementService
    {
      public  Task<ApiResponse<GetAnnouncementCountQueryResponse>> GetAnnouncementCountAsync(GetAnnouncementCountQueryRequest request, CancellationToken cancellationToken = default);
      public  Task<ApiResponse<IList<GetAnnouncementCountByTypeQueryResponse>>> GetAnnouncementCountByTypeAsync(GetAnnouncementCountByTypeQueryRequest request, CancellationToken cancellationToken = default);
      public  Task<ApiResponse<IList<GetAnnouncementCountByProvinceQueryResponse>>> GetAnnouncementCountByProvinceAsync(GetAnnouncementCountByProvinceQueryRequest request, CancellationToken cancellationToken = default);
      public  Task<ApiResponse<GetTodaysAnnouncementsQueryResponse>> GetTodaysAnnouncementsAsync(GetTodaysAnnouncementsQueryRequest request, CancellationToken cancellationToken = default);
      public  Task<ApiResponse<SearchAnnouncementsQueryResponse>> SearchAnnouncementsAsync(SearchAnnouncementsQueryRequest request, CancellationToken cancellationToken = default);
        Task<ApiResponse<List<GetAnnouncementsByCompanyQueryResponse>>> GetAnnouncementsByCompanyAsync(GetAnnouncementsByCompanyQueryRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<List<GetAnnouncementsByUserQueryResponse>>> GetAnnouncementsByUserAsync(GetAnnouncementsByUserQueryRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<IList<GetCompanyAnnouncementCountByProvinceQueryResponse>>> GetCompanyAnnouncementCountByProvinceAsync(GetCompanyAnnouncementCountByProvinceQueryRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<GetAnnouncementDetailsQueryResponse>> GetAnnouncementDetailsAsync(GetAnnouncementDetailsQueryRequest request, CancellationToken cancellationToken);
    }
}
