using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Queries.GetAllUsers
{

    public class GetAllUsersQueryHandler : BaseHandler, IRequestHandler<GetAllUsersQueryRequest, ApiResponse<IList<GetAllUsersQueryResponse>>>
    {
        private readonly IUserService _userService;

        public GetAllUsersQueryHandler(IHttpContextAccessor httpContextAccessor, IUserService userService) : base(httpContextAccessor)
        {

            _userService = userService;

        }
        public async Task<ApiResponse<IList<GetAllUsersQueryResponse>>> Handle(GetAllUsersQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _userService.GetAllUsers(request);

            return response;
        }
    }
}
