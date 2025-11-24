using FluentValidation;

namespace Sevval.Application.Features.Auth.Queries.Auth;

public class AuthQueryValidator : AbstractValidator<AuthQueryRequest>
{
    public AuthQueryValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta adresi zorunludur!");
        RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre zorunludur!");
    }
}
