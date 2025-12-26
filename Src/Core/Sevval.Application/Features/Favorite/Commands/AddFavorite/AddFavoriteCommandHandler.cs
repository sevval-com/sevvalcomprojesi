using MediatR;
using Sevval.Application.Abstractions.Services;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Favorite.Commands.AddFavorite;

public class AddFavoriteCommandHandler : IRequestHandler<AddFavoriteCommandRequest, ApiResponse<AddFavoriteCommandResponse>>
{
    private readonly IFavoriteService _favoriteService;

    public AddFavoriteCommandHandler(IFavoriteService favoriteService)
    {
        _favoriteService = favoriteService;
    }

    public async Task<ApiResponse<AddFavoriteCommandResponse>> Handle(AddFavoriteCommandRequest request, CancellationToken cancellationToken)
    {
       
            var result = await _favoriteService.AddFavoriteAsync(request, cancellationToken);
            return result;
        
      
    }
}
