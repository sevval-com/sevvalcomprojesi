using FluentValidation;

namespace Sevval.Application.Features.InvestmentRequest.Commands.CreateInvestmentRequest;

public class CreateInvestmentRequestCommandValidator : AbstractValidator<CreateInvestmentRequestCommandRequest>
{
    public CreateInvestmentRequestCommandValidator()
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
            
            .MaximumLength(20)
            .WithMessage("Telefon numarası en fazla 20 karakter olabilir.");

        //// MinBudget and MaxBudget validation
        //When(x => !string.IsNullOrEmpty(x.MinBudget), () => {
        //    RuleFor(x => x.MinBudget)
        //        .Matches(@"^[0-9]+$")
        //        .WithMessage("Minimum bütçe sadece sayı içermelidir.")
        //        .MaximumLength(20)
        //        .WithMessage("Minimum bütçe en fazla 20 karakter olabilir.");
        //});

        //When(x => !string.IsNullOrEmpty(x.MaxBudget), () => {
        //    RuleFor(x => x.MaxBudget)
        //        .Matches(@"^[0-9]+$")
        //        .WithMessage("Maksimum bütçe sadece sayı içermelidir.")
        //        .MaximumLength(20)
        //        .WithMessage("Maksimum bütçe en fazla 20 karakter olabilir.");
        //});

        // Optional field validations with length limits
        When(x => !string.IsNullOrEmpty(x.Area), () => {
            RuleFor(x => x.Area)
                .MaximumLength(50)
                .WithMessage("Alan bilgisi en fazla 50 karakter olabilir.");
        });

        When(x => !string.IsNullOrEmpty(x.SelectCategory), () => {
            RuleFor(x => x.SelectCategory)
                .MaximumLength(100)
                .WithMessage("Kategori en fazla 100 karakter olabilir.");
        });

        When(x => !string.IsNullOrEmpty(x.SelectStatus), () => {
            RuleFor(x => x.SelectStatus)
                .MaximumLength(100)
                .WithMessage("Durum en fazla 100 karakter olabilir.");
        });

        When(x => !string.IsNullOrEmpty(x.ResidentialCity), () => {
            RuleFor(x => x.ResidentialCity)
                .MaximumLength(100)
                .WithMessage("Şehir en fazla 100 karakter olabilir.");
        });

        When(x => !string.IsNullOrEmpty(x.ResidentialDistrict), () => {
            RuleFor(x => x.ResidentialDistrict)
                .MaximumLength(100)
                .WithMessage("İlçe en fazla 100 karakter olabilir.");
        });
    }
}
