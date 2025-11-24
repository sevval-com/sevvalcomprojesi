using FluentValidation;

namespace Sevval.Application.Features.User.Commands.ForgottenPassword;


public class ForgottenPasswordCommandValidator : AbstractValidator<ForgottenPasswordCommandRequest>
{
    public ForgottenPasswordCommandValidator()
    {
        RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta adresi zorunludur");
        RuleFor(x => x.Code).NotEmpty().WithMessage("Doğrulama kodu zorunludur");
        RuleFor(x => x.NewPassword).NotEmpty().WithMessage("Yeni şifre zorunludur")
            .MinimumLength(8).WithMessage("Yeni şifre en az 6 karakter olmalıdır")
            .MaximumLength(100).WithMessage("Yeni şifre 100 karakteri geçemez");
    }
}
