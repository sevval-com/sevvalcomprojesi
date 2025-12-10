using MediatR;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Admin.Commands.ApproveUser;

public class ApproveUserCommandRequest : IRequest<ApiResponse<ApproveUserCommandResponse>>
{
    public string UserId { get; set; } = string.Empty;
    public bool Approved { get; set; }
}
