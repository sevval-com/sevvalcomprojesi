using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.User.Commands.CorporateRegister;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.ForgottenPassword;

public class ForgottenPasswordCommandHandler : BaseHandler, IRequestHandler<ForgottenPasswordCommandRequest, ApiResponse<ForgottenPasswordCommandResponse>>
{
    private readonly IUserService _userService;

    public ForgottenPasswordCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService) : base(httpContextAccessor)
    {

        _userService = userService;

    }
    public async Task<ApiResponse<ForgottenPasswordCommandResponse>> Handle(ForgottenPasswordCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _userService.ForgotPassword(request);

        return response;
    }
}
