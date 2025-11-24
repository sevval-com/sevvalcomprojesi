using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.User.Commands.UpdateUser;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.UpdateUserProfile
{
     
    public class UpdateUserProfileCommandHandler : BaseHandler, IRequestHandler<UpdateUserProfileCommandRequest, ApiResponse<UpdateUserProfileCommandResponse>>
    {
        private readonly IUserService _userService;

        public UpdateUserProfileCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService) : base(httpContextAccessor)
        {

            _userService = userService;

        }
        public async Task<ApiResponse<UpdateUserProfileCommandResponse>> Handle(UpdateUserProfileCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.UpdateUserProfile(request, cancellationToken);

            return response;
        }
    }
}
