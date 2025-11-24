using MediatR;
using Sevval.Application.Features.Common;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sevval.Application.Features.RecentlyVisitedAnnouncement.Commands.AddRecentlyVisitedAnnouncement
{
    public class AddRecentlyVisitedAnnouncementCommandRequest : IRequest<ApiResponse<AddRecentlyVisitedAnnouncementCommandResponse>>
    {
        public const string Route = "/api/v1/recently-visited-announcements";

        [NotMapped]
        [SwaggerIgnore]
        public string UserId { get; set; }
        public int AnnouncementId { get; set; }
        public string Province { get; set; } //ilanýn sehir bilgisi
        public string Property { get; set; } //ilanýn türü, arsa,konut,...

    }
}
