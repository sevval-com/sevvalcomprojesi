using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.User.Commands.ForgottenPassword
{
    public class ForgottenPasswordCommandRequest : IRequest<ApiResponse<ForgottenPasswordCommandResponse>>
    {
        public const string Route = "/api/v1/forgotten-password";

        public string Email { get; set; }
        public string Code { get; set; }  // Doğrulama kodu
        public string NewPassword { get; set; }  // Yeni şifre
    }
}
