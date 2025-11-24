using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByCompany
{
    public class GetAnnouncementsByCompanyQueryRequest : IRequest<ApiResponse<List<GetAnnouncementsByCompanyQueryResponse>>>
    {
        public const string Route = "/api/v1/announcements/company";

        public string UserId { get; set; }
        public string? CompanyName { get; set; }
        public string? Status { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }

    }
}
