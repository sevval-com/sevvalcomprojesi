using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.CorporateUpdate;

public class CorporateUpdateCommandHandler : IRequestHandler<CorporateUpdateCommandRequest, ApiResponse<CorporateUpdateCommandResponse>>
{
    private readonly IUserService _userService;

    public CorporateUpdateCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResponse<CorporateUpdateCommandResponse>> Handle(CorporateUpdateCommandRequest request, CancellationToken cancellationToken)
    {
        return await _userService.CorporateUpdate(request, cancellationToken);
    }
}
