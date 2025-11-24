using MediatR;
using Microsoft.AspNetCore.Mvc;
using Sevval.Application.Features.Common;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json.Serialization;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementDetails
{
    public class GetAnnouncementDetailsQueryRequest : IRequest<ApiResponse<GetAnnouncementDetailsQueryResponse>>
    {
        public const string Route = "/api/v1/announcements/{id}";

        [FromRoute]
        public int Id { get; set; }

        [SwaggerIgnore]
        public string? UserEmail { get; set; } // For access control
    }
}
