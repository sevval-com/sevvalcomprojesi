using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Role.Queries.GetUserRoles
{
    public class GetUserRolesQueryRequest : IRequest<ApiResponse<IList<GetUserRolesQueryResponse>>>
    {
        public const string Route = "/api/v1/user/roles";

    }
}
