using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.UpdatePassword;

public class UpdatePasswordCommandHandler : IRequestHandler<UpdatePasswordCommandRequest, ApiResponse<UpdatePasswordCommandResponse>>
{
    private readonly IUserService _userService;

    public UpdatePasswordCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResponse<UpdatePasswordCommandResponse>> Handle(UpdatePasswordCommandRequest request, CancellationToken cancellationToken)
    {
        return await _userService.UpdatePassword(request, cancellationToken);
    }
}
