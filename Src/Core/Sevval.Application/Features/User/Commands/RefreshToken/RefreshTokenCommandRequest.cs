using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.User.Commands.RefreshToken
{
    public class RefreshTokenCommandRequest : IRequest<ApiResponse<RefreshTokenCommandResponse>>
    {
        public const string Route = "/api/v1/auth/refresh-token";

        public string RefreshToken { get; set; }
    }
}
