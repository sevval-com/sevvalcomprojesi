using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.User.Queries.GetUserById;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.DeleteUser
{
  
    public class DeleteUserCommandHandler : BaseHandler, IRequestHandler<DeleteUserCommandRequest, ApiResponse<DeleteUserCommandResponse>>
    {
        private readonly IUserService _userService;

        public DeleteUserCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService) : base(httpContextAccessor)
        {

            _userService = userService;

        }
        public async Task<ApiResponse<DeleteUserCommandResponse>> Handle(DeleteUserCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.DeleteUser(request, cancellationToken);

            return response;
        }
    }
}
