using MediatR;
using Sevval.Application.Features.Common;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sevval.Application.Features.Announcement.Queries.GetSuitableAnnouncements
{
    public class GetSuitableAnnouncementsQueryRequest : IRequest<ApiResponse<List<GetSuitableAnnouncementsQueryResponse>>>
    {
        public const string Route = "/api/v1/announcements/suitable";

        [NotMapped]
        [SwaggerIgnore]
        public string UserId { get; set; }
        public int Page { get; set; }
        public int Size { get; set; }


    }
}
