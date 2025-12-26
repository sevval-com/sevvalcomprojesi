using FluentValidation;

namespace Sevval.Application.Features.TempEstateVerification.Commands.CreateTempEstateVerification
{
    public class CreateTempEstateVerificationCommandValidator : AbstractValidator<CreateTempEstateVerificationCommandRequest>
    {
        public CreateTempEstateVerificationCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta gereklidir")
                .EmailAddress().WithMessage("Geçersiz e-posta adresi");
        }
    }
}
