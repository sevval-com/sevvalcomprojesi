using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.User.Commands.SendNewCode;

public class SendNewCodeCommandRequest:IRequest<ApiResponse<SendNewCodeCommandResponse>>
{
    public const string Route = "/api/v1/user/send-new-code";

    public string Email { get; set; }
}
