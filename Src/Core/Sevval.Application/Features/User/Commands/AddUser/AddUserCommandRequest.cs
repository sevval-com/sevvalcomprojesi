using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.User.Commands.AddUser
{
    public class AddUserCommandRequest : IRequest<ApiResponse<AddUserCommandResponse>>
    {
        public const string Route = "/api/v1/users";

        public string Email { get; set; }
        public string Password { get; set; }
        public string UserType { get; set; }

    }
}
