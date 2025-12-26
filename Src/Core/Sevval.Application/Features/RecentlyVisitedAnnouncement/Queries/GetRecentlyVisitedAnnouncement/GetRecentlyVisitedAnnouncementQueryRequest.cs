using MediatR;
using Sevval.Application.Features.Common;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace Sevval.Application.Features.RecentlyVisitedAnnouncement.Queries.GetRecentlyVisitedAnnouncement
{
    public class GetRecentlyVisitedAnnouncementQueryRequest : IRequest<ApiResponse<List<GetRecentlyVisitedAnnouncementQueryResponse>>>
    {
        public const string Route = "/api/v1/recently-visited-announcements";

        [NotMapped]
        [SwaggerIgnore]
        public string UserId { get; set; }
        public int Page { get; set; }  
        public int Size { get; set; } 
    }
}
