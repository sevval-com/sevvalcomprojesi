using FluentValidation;

namespace Sevval.Application.Features.User.Commands.ConfirmEstate
{
    public class ConfirmEstateCommandValidator : AbstractValidator<ConfirmEstateCommandRequest>
    {
        public ConfirmEstateCommandValidator()
        {
            RuleFor(x => x.Token).NotEmpty().WithMessage("Token gereklidir");
            RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta gereklidir")
                .EmailAddress().WithMessage("Geçersiz e-posta adresi");
        }
    }
}
