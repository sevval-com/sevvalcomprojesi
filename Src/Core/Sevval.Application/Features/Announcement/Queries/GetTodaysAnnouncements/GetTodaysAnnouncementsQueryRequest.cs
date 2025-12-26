using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Announcement.Queries.GetTodaysAnnouncements
{
    public class GetTodaysAnnouncementsQueryRequest : IRequest<ApiResponse<GetTodaysAnnouncementsQueryResponse>>
    {
        public const string Route = "/api/v1/announcements/todays";
        public string? Status { get; set; } = "active"; // Default to active announcements
        
        /// <summary>
        /// Benzersiz cihaz kimliği (opsiyonel). Gönderilirse aynı cihaz 24 saat içinde tekrar sayılmaz.
        /// </summary>
        public string? DeviceId { get; set; }
    }
}
