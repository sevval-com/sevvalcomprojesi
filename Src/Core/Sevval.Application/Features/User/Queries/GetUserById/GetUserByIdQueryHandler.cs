using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Queries.GetUserById;


public class GetUserByIdQueryHandler : BaseHandler, IRequestHandler<GetUserByIdQueryRequest, ApiResponse<GetUserByIdQueryResponse>>
{
    private readonly IUserService _userService;

    public GetUserByIdQueryHandler(IHttpContextAccessor httpContextAccessor, IUserService userService) : base(httpContextAccessor)
    {

        _userService = userService;

    }
    public async Task<ApiResponse<GetUserByIdQueryResponse>> Handle(GetUserByIdQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _userService.GetUser(request);

        return response;
    }
}
