using FluentValidation;

namespace Sevval.Application.Features.User.Commands.IndividualRegister;

public class IndividualRegisterCommandValidator : AbstractValidator<IndividualRegisterCommandRequest>
{
    public IndividualRegisterCommandValidator()
    {

        RuleFor(x => x.FirstName)
       .NotEmpty().WithMessage("Ad alanı gereklidir.");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad alanı gereklidir.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("E-posta alanı gereklidir.")
            .EmailAddress().WithMessage("Geçerli bir e-posta adresi girin.");

        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Telefon numarası alanı gereklidir.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre alanı gereklidir.")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter uzunluğunda olmalıdır.")
            .MaximumLength(100).WithMessage("Şifre en fazla 100 karakter uzunluğunda olmalıdır.");

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty().WithMessage("Şifreyi onaylama alanı gereklidir.")
            .Equal(x => x.Password).WithMessage("Şifreler eşleşmiyor.");
                
    }

}

