using FluentValidation;
using Microsoft.AspNetCore.Http;

namespace Sevval.Application.Features.User.Commands.CorporateRegister
{
    public class CorporateRegisterCommandValidator : AbstractValidator<CorporateRegisterCommandRequest>
    {
        public CorporateRegisterCommandValidator()
        {

            RuleFor(x => x.UserTypes).NotEmpty().WithMessage("Kullanıcı tipi gereklidir");
            RuleFor(x => x.UserTypes).Must(x => allowedUserTypes.Contains(x)).WithMessage("Kullanıcı tipi geçersizdir");
            RuleFor(x => x.FirstName).NotEmpty().WithMessage("Ad gereklidir");
            RuleFor(x => x.LastName).NotEmpty().WithMessage("Soyad gereklidir");
            RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta gereklidir").EmailAddress().WithMessage("Geçersiz e-posta adresi");
            RuleFor(x => x.PhoneNumber).NotEmpty().WithMessage("Telefon numarası gereklidir").WithMessage("Geçersiz telefon numarası");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Parola gereklidir").MinimumLength(8).WithMessage("Parola en az 8 karakter olmalıdır");
            RuleFor(x => x.ConfirmPassword).NotEmpty().WithMessage("Parola tekrarı gereklidir").Equal(x => x.Password).WithMessage("Parolalar eşleşmiyor");
            RuleFor(x => x.CompanyName).NotEmpty().WithMessage("Şirket adı gereklidir");
            RuleFor(x => x.City).NotEmpty().WithMessage("Şehir gereklidir");
            RuleFor(x => x.District).NotEmpty().WithMessage("İlçe gereklidir");
            RuleFor(x => x.Address).NotEmpty().WithMessage("Adres gereklidir");
            RuleFor(x => x.Level5Certificate).NotNull().WithMessage("Seviye 5 sertifikası gereklidir").SetValidator(new FileValidator());
            RuleFor(x => x.TaxPlate).NotNull().WithMessage("Vergi levhası gereklidir").SetValidator(new FileValidator());
            RuleFor(x => x.ProfilePicture).SetValidator(new FileValidator()).When(x => x.ProfilePicture != null);


        }
        private readonly string[] allowedUserTypes = new[]
{
    
    "Kurumsal",
    "İnşaat",
    "Vakıf",
    "Banka"
};

    }
}
public class FileValidator : AbstractValidator<IFormFile>
{
    public FileValidator()
    {
        RuleFor(x => x.Length).GreaterThan(0).WithMessage("Dosya gereklidir");
        RuleFor(x => x.FileName).NotEmpty().WithMessage("Dosya adı gereklidir");
        //RuleFor(x => x.ContentType).Must(x => x.StartsWith("image/")).WithMessage("Sadece resim dosyaları kabul edilir");
    }
}
