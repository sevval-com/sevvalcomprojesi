using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.User.Queries.GetUserById;

namespace Sevval.Application.Features.User.Commands.UpdateUser
{
    public class UpdateUserCommandRequest : IRequest<ApiResponse<UpdateUserCommandResponse>>
    {
        public const string Route = "/api/v1/users";
        public string Id { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
