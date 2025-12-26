using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Role.Queries.GetUserRoles;


public class GetUserRolesQueryHandler : BaseHandler, IRequestHandler<GetUserRolesQueryRequest, ApiResponse<IList<GetUserRolesQueryResponse>>>
{
    private readonly IRoleService _roleService;

    public GetUserRolesQueryHandler(IHttpContextAccessor httpContextAccessor,  IRoleService roleService) : base(httpContextAccessor)
    {
        _roleService = roleService;
    }

    public async Task<ApiResponse<IList<GetUserRolesQueryResponse>>> Handle(GetUserRolesQueryRequest request, CancellationToken cancellationToken)
    {
        var response = await _roleService.GetUserRoles(request);

        return response;
    }
}
