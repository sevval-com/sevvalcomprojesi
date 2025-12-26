using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.RefreshToken
{
    public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommandRequest, ApiResponse<RefreshTokenCommandResponse>>
    {
        private readonly IUserService _userService;

        public RefreshTokenCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<ApiResponse<RefreshTokenCommandResponse>> Handle(RefreshTokenCommandRequest request, CancellationToken cancellationToken)
        {
            return await _userService.RefreshToken(request, cancellationToken);
        }
    }
}
