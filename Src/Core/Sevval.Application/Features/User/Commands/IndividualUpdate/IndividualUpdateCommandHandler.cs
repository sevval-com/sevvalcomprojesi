using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.IndividualUpdate;

public class IndividualUpdateCommandHandler : IRequestHandler<IndividualUpdateCommandRequest, ApiResponse<IndividualUpdateCommandResponse>>
{
    private readonly IUserService _userService;

    public IndividualUpdateCommandHandler(IUserService userService)
    {
        _userService = userService;
    }

    public async Task<ApiResponse<IndividualUpdateCommandResponse>> Handle(IndividualUpdateCommandRequest request, CancellationToken cancellationToken)
    {
        return await _userService.IndividualUpdate(request, cancellationToken);
    }
}
