using Sevval.Application.Features.Common;
using Sevval.Application.Features.Favorite.Commands.AddFavorite;

namespace Sevval.Application.Abstractions.Services;

public interface IFavoriteService
{
    Task<ApiResponse<AddFavoriteCommandResponse>> AddFavoriteAsync(AddFavoriteCommandRequest request, CancellationToken cancellationToken);
}
