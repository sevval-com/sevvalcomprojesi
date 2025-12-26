using FluentValidation;

namespace Sevval.Application.Features.SalesRequest.Commands.CreateSalesRequest;

public class CreateSalesRequestCommandValidator : AbstractValidator<CreateSalesRequestCommandRequest>
{
    public CreateSalesRequestCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("Ad alanı zorunludur.")
            .MaximumLength(100)
            .WithMessage("Ad en fazla 100 karakter olabilir.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Soyad alanı zorunludur.")
            .MaximumLength(100)
            .WithMessage("Soyad en fazla 100 karakter olabilir.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("E-posta adresi zorunludur.")
            .EmailAddress()
            .WithMessage("Geçerli bir e-posta adresi giriniz.")
            .MaximumLength(200)
            .WithMessage("E-posta adresi en fazla 200 karakter olabilir.");

        RuleFor(x => x.Phone)
            .NotEmpty()
            .WithMessage("Telefon numarası zorunludur.")
            .Matches(@"^[0-9\s\-\+\(\)]+$")
            .WithMessage("Geçerli bir telefon numarası giriniz.")
            .MaximumLength(20)
            .WithMessage("Telefon numarası en fazla 20 karakter olabilir.");

        //RuleFor(x => x.SelectCategory)
        //    .NotEmpty()
        //    .WithMessage("Kategori seçimi zorunludur.");

        //RuleFor(x => x.SelectStatus)
        //    .NotEmpty()
        //    .WithMessage("Durum seçimi zorunludur.");

        //// Optional field validations with length limits
        //When(x => !string.IsNullOrEmpty(x.Area), () => {
        //    RuleFor(x => x.Area)
        //        .MaximumLength(50)
        //        .WithMessage("Alan bilgisi en fazla 50 karakter olabilir.");
        //});

        //When(x => !string.IsNullOrEmpty(x.Price), () => {
        //    RuleFor(x => x.Price)
        //        .MaximumLength(50)
        //        .WithMessage("Fiyat bilgisi en fazla 50 karakter olabilir.");
        //});

        //When(x => !string.IsNullOrEmpty(x.LandPrice), () => {
        //    RuleFor(x => x.LandPrice)
        //        .MaximumLength(50)
        //        .WithMessage("Arsa fiyat bilgisi en fazla 50 karakter olabilir.");
        //});

        //When(x => !string.IsNullOrEmpty(x.ResidentialCity), () => {
        //    RuleFor(x => x.ResidentialCity)
        //        .MaximumLength(100)
        //        .WithMessage("Konut şehir bilgisi en fazla 100 karakter olabilir.");
        //});

        //When(x => !string.IsNullOrEmpty(x.LandCity), () => {
        //    RuleFor(x => x.LandCity)
        //        .MaximumLength(100)
        //        .WithMessage("Arsa şehir bilgisi en fazla 100 karakter olabilir.");
        //});

        //When(x => !string.IsNullOrEmpty(x.LivingCity), () => {
        //    RuleFor(x => x.LivingCity)
        //        .MaximumLength(100)
        //        .WithMessage("Yaşadığı şehir bilgisi en fazla 100 karakter olabilir.");
        //});
    }
}
