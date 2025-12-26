using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.User.Queries.CheckUserExists;

public class CheckUserExistsQueryRequest : IRequest<ApiResponse<CheckUserExistsQueryResponse>>
{
    public const string Route = "/api/v1/check-users-exists";
    public string Email { get; set; }
    public string Phone { get; set; }
}
