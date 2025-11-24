using FluentValidation;

namespace Sevval.Application.Features.User.Commands.IndividualUpdate;

public class IndividualUpdateCommandValidator : AbstractValidator<IndividualUpdateCommandRequest>
{
    public IndividualUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Kullanıcı ID'si gereklidir.");

        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Ad alanı gereklidir.")
            .MaximumLength(50)
            .WithMessage("Ad en fazla 50 karakter olabilir.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Soyad alanı gereklidir.")
            .MaximumLength(50)
            .WithMessage("Soyad en fazla 50 karakter olabilir.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("E-posta alanı gereklidir.")
            .EmailAddress()
            .WithMessage("Geçerli bir e-posta adresi giriniz.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty()
            .WithMessage("Telefon numarası gereklidir.");

        // Password validation only if password is provided
        When(x => !string.IsNullOrEmpty(x.Password), () => {
            RuleFor(x => x.Password)
                .MinimumLength(6)
                .WithMessage("Şifre en az 6 karakter olmalıdır.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Şifreler eşleşmiyor.");
        });
    }
}
