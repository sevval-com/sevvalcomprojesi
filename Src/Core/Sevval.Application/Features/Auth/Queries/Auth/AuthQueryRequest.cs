using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Auth.Queries.Auth
{
    public class AuthQueryRequest : IRequest<ApiResponse<AuthQueryResponse>>
    {
        public const string Route = "/api/v1/auth";
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
