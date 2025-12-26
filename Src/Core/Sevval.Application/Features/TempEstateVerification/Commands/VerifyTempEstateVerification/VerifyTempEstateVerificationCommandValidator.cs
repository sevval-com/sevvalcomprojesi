using FluentValidation;

namespace Sevval.Application.Features.TempEstateVerification.Commands.VerifyTempEstateVerification
{
    public class VerifyTempEstateVerificationCommandValidator : AbstractValidator<VerifyTempEstateVerificationCommandRequest>
    {
        public VerifyTempEstateVerificationCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta gereklidir")
                .EmailAddress().WithMessage("Geçersiz e-posta adresi");
            
            RuleFor(x => x.Code).NotEmpty().WithMessage("Doğrulama kodu gereklidir")
                .Length(6).WithMessage("Doğrulama kodu 6 karakter olmalıdır");
        }
    }
}
