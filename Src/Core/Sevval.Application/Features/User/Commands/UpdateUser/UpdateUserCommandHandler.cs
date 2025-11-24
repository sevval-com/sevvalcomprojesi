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

namespace Sevval.Application.Features.User.Commands.UpdateUser
{
    
    public class UpdateUserCommandHandler : BaseHandler, IRequestHandler<UpdateUserCommandRequest, ApiResponse<UpdateUserCommandResponse>>
    {
        private readonly IUserService _userService;

        public UpdateUserCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService) : base(httpContextAccessor)
        {

            _userService = userService;

        }
        public async Task<ApiResponse<UpdateUserCommandResponse>> Handle(UpdateUserCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.UpdateUser(request, cancellationToken);

            return response;
        }
    }
}
