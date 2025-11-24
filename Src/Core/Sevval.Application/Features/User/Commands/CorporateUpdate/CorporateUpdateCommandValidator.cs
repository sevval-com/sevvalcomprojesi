using FluentValidation;

namespace Sevval.Application.Features.User.Commands.CorporateUpdate;

public class CorporateUpdateCommandValidator : AbstractValidator<CorporateUpdateCommandRequest>
{
    public CorporateUpdateCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage("Kullanıcı ID'si gereklidir.");

        RuleFor(x => x.UserTypes)
            .NotEmpty()
            .WithMessage("Kullanıcı tipi gereklidir.")
            .Must(x => new[] { "Kurumsal", "Vakıf", "İnşaat", "Banka" }.Contains(x))
            .WithMessage("Geçerli bir kullanıcı tipi seçiniz.");

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

        RuleFor(x => x.CompanyName)
            .NotEmpty()
            .WithMessage("Şirket adı gereklidir.")
            .MaximumLength(100)
            .WithMessage("Şirket adı en fazla 100 karakter olabilir.");

        RuleFor(x => x.City)
            .NotEmpty()
            .WithMessage("Şehir gereklidir.");

        RuleFor(x => x.District)
            .NotEmpty()
            .WithMessage("İlçe gereklidir.");

        RuleFor(x => x.Address)
            .NotEmpty()
            .WithMessage("Adres gereklidir.")
            .MaximumLength(500)
            .WithMessage("Adres en fazla 500 karakter olabilir.");

        // Password validation only if password is provided
        When(x => !string.IsNullOrEmpty(x.Password), () => {
            RuleFor(x => x.Password)
                .MinimumLength(8)
                .WithMessage("Şifre en az 6 karakter olmalıdır.");

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .WithMessage("Şifreler eşleşmiyor.");
        });
    }
}
