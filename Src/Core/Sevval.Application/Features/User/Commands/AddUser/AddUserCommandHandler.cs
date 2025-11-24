using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.AddUser
{


    public class AddUserCommandHandler : BaseHandler, IRequestHandler<AddUserCommandRequest, ApiResponse<AddUserCommandResponse>>
    {
        private readonly IUserService _userService;

        public AddUserCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService) : base(httpContextAccessor)
        {

            _userService = userService;

        }
        public async Task<ApiResponse<AddUserCommandResponse>> Handle(AddUserCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.AddUser(request, cancellationToken);

            return response;
        }
    }
}
