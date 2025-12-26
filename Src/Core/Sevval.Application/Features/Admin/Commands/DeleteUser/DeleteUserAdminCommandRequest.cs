using MediatR;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Admin.Commands.DeleteUser;

public class DeleteUserAdminCommandRequest : IRequest<ApiResponse<DeleteUserAdminCommandResponse>>
{
    public string UserId { get; set; } = string.Empty;
}
