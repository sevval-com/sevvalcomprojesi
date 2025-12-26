using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.LoginWithSocialMedia;

public class LoginWithSocialMediaCommandHandler : IRequestHandler<LoginWithSocialMediaCommandRequest, ApiResponse<LoginWithSocialMediaCommandResponse>>
{
    private readonly IUserService _userService;

    public LoginWithSocialMediaCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResponse<LoginWithSocialMediaCommandResponse>> Handle(LoginWithSocialMediaCommandRequest request, CancellationToken cancellationToken)
    {
        return await _userService.LoginWithSocialMedia(request, cancellationToken);
    }
}
