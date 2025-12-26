using FluentValidation;
using System.Text.RegularExpressions;

namespace Sevval.Application.Features.User.Commands.LoginWithSocialMedia;

public class LoginWithSocialMediaCommandValidator : AbstractValidator<LoginWithSocialMediaCommandRequest>
{
    private readonly string[] allowedUserTypes = new[]
    {
        "Bireysel",
        "Kurumsal",
        "Vakýf",
        "Ýnþaat",
        "Banka"
    };

    public LoginWithSocialMediaCommandValidator()
    {
        RuleFor(x => x.Provider)
            .NotEmpty().WithMessage("Provider alaný boþ olamaz")
            .MaximumLength(50).WithMessage("Provider alaný 50 karakterden fazla olamaz");

        RuleFor(x => x.Token)
            .MaximumLength(2000).WithMessage("Token 2000 karakterden fazla olamaz");

        RuleFor(x => x.SocialId)
            .NotEmpty().WithMessage("SocialId alaný boþ olamaz")
            .MaximumLength(100).WithMessage("SocialId alaný 100 karakterden fazla olamaz");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email alaný boþ olamaz")
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz")
            .MaximumLength(100).WithMessage("Email alaný 100 karakterden fazla olamaz");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("Ad alaný boþ olamaz")
            .MaximumLength(50).WithMessage("Ad alaný 50 karakterden fazla olamaz");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Soyad alaný boþ olamaz")
            .MaximumLength(50).WithMessage("Soyad alaný 50 karakterden fazla olamaz");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(20).WithMessage("Telefon numarasý 20 karakterden fazla olamaz")
            ;

        RuleFor(x => x.PhotoUrl)
            .MaximumLength(2000).WithMessage("Fotoðraf URL'i 2000 karakterden fazla olamaz")
            .Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.PhotoUrl))
            .WithMessage("Geçerli bir URL giriniz");

        RuleFor(x => x.UserType)
            .NotEmpty().WithMessage("Kullanýcý tipi boþ olamaz")
            .Must(BeValidUserType).WithMessage($"Kullanýcý tipi þunlardan biri olmalýdýr: {string.Join(", ", allowedUserTypes)}");
    }

    

    private bool BeValidUrl(string url)
    {
        if (string.IsNullOrEmpty(url))
            return true;

        return Uri.TryCreate(url, UriKind.Absolute, out _);
    }

    private bool BeValidUserType(string userType)
    {
        var dd= allowedUserTypes.Contains(userType);
        return dd;
    }
}