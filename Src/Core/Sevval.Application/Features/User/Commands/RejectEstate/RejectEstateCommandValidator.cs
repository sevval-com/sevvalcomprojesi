using FluentValidation;

namespace Sevval.Application.Features.User.Commands.RejectEstate
{
    public class RejectEstateCommandValidator : AbstractValidator<RejectEstateCommandRequest>
    {
        public RejectEstateCommandValidator()
        {
            RuleFor(x => x.Token).NotEmpty().WithMessage("Token gereklidir");
            RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta gereklidir")
                .EmailAddress().WithMessage("Geçersiz e-posta adresi");
        }
    }
}
