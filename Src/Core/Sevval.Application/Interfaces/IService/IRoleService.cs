using Sevval.Application.Features.Common;
using Sevval.Application.Features.Role.Queries.GetUserRoles;

namespace Sevval.Application.Interfaces.IService
{
    public interface IRoleService
    {
        Task<ApiResponse<IList<GetUserRolesQueryResponse>>> GetUserRoles(GetUserRolesQueryRequest request);
    }
}
