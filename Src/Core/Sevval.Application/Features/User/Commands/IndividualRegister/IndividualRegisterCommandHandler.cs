using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.IndividualRegister;


public class IndividualRegisterCommandHandler : BaseHandler, IRequestHandler<IndividualRegisterCommandRequest, ApiResponse<IndividualRegisterCommandResponse>>
{
    private readonly IUserService _userService;

    public IndividualRegisterCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService) : base(httpContextAccessor)
    {

        _userService = userService;

    }
    public async Task<ApiResponse<IndividualRegisterCommandResponse>> Handle(IndividualRegisterCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _userService.IndividualRegister(request, cancellationToken);

        return response;
    }
}
