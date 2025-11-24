using FluentValidation;

namespace Sevval.Application.Features.User.Commands.RefreshToken
{
    public class RefreshTokenCommandValidator : AbstractValidator<RefreshTokenCommandRequest>
    {
        public RefreshTokenCommandValidator()
        {
            RuleFor(x => x.RefreshToken)
                .NotEmpty().WithMessage("Refresh token gereklidir")
                .NotNull().WithMessage("Refresh token boş olamaz");
        }
    }
}
