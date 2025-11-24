using FluentValidation;

namespace Sevval.Application.Features.TempEstateVerification.Queries.GetTempEstateVerification
{
    public class GetTempEstateVerificationQueryValidator : AbstractValidator<GetTempEstateVerificationQueryRequest>
    {
        public GetTempEstateVerificationQueryValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta gereklidir")
                .EmailAddress().WithMessage("Geçersiz e-posta adresi");
        }
    }
}
