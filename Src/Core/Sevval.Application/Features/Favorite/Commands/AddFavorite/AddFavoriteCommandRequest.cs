using MediatR;
using Sevval.Application.Features.Common;
using Swashbuckle.AspNetCore.Annotations;

namespace Sevval.Application.Features.Favorite.Commands.AddFavorite;

public class AddFavoriteCommandRequest : IRequest<ApiResponse<AddFavoriteCommandResponse>>
{
    public const string Route = "/api/v1/favorites";

    public int AnnouncementId { get; set; }


    [SwaggerIgnore]
    public string UserEmail { get; set; }

    [SwaggerIgnore]
    public string IpAddress { get; set; }
}
