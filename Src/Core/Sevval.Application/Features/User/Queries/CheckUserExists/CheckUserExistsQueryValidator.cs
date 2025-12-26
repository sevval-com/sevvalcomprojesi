using FluentValidation;

namespace Sevval.Application.Features.User.Queries.CheckUserExists
{
    public class CheckUserExistsQueryRequestValidator : AbstractValidator<CheckUserExistsQueryRequest>
    {
        public CheckUserExistsQueryRequestValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("E-posta adresi gereklidir")
                .EmailAddress()
                .WithMessage("Geçersiz e-posta adresi");

            RuleFor(x => x.Phone)
                .NotEmpty()
                .WithMessage("Telefon numarası gereklidir")
                .WithMessage("Geçersiz telefon numarası formatı");
        }
    }
}
