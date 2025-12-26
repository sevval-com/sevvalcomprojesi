using FluentValidation;

namespace Sevval.Application.Features.User.Commands.UpdatePassword;

public class UpdatePasswordCommandValidator : AbstractValidator<UpdatePasswordCommandRequest>
{
    public UpdatePasswordCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty()
            .WithMessage("Kullanıcı ID'si gereklidir.");

        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .WithMessage("Mevcut şifre gereklidir.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .WithMessage("Yeni şifre gereklidir.")
            .MinimumLength(6)
            .WithMessage("Yeni şifre en az 6 karakter olmalıdır.")
            .Matches(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)")
            .WithMessage("Yeni şifre en az bir küçük harf, bir büyük harf ve bir rakam içermelidir.");

        RuleFor(x => x.ConfirmNewPassword)
            .NotEmpty()
            .WithMessage("Yeni şifre tekrarı gereklidir.")
            .Equal(x => x.NewPassword)
            .WithMessage("Yeni şifre ve tekrarı eşleşmiyor.");
    }
}
