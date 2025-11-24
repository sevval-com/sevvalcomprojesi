using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.SendNewCode;


public class SendNewCodeCommandHandler : BaseHandler, IRequestHandler<SendNewCodeCommandRequest, ApiResponse<SendNewCodeCommandResponse>>
{
    private readonly IUserService _userService;

    public SendNewCodeCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService) : base(httpContextAccessor)
    {

        _userService = userService;

    }
    public async Task<ApiResponse<SendNewCodeCommandResponse>> Handle(SendNewCodeCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _userService.SendNewCode(request, cancellationToken);

        return response;
    }
}
