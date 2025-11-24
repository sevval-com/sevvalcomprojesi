using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;
namespace Sevval.Application.Features.User.Queries.CheckUserExists;


public class CheckUserExistsQueryHandler : BaseHandler, IRequestHandler<CheckUserExistsQueryRequest, ApiResponse<CheckUserExistsQueryResponse>>
{
    private readonly IUserService _userService;

    public CheckUserExistsQueryHandler(IHttpContextAccessor httpContextAccessor, IUserService userService) : base(httpContextAccessor)
    {

        _userService = userService;

    }
    public async Task<ApiResponse<CheckUserExistsQueryResponse>> Handle(CheckUserExistsQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _userService.CheckUserExists(request);

        return response;
    }
}
