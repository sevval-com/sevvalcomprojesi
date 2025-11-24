using FluentValidation;

namespace Sevval.Application.Features.TempEstateVerification.Commands.DeleteTempEstateVerification
{
    public class DeleteTempEstateVerificationCommandValidator : AbstractValidator<DeleteTempEstateVerificationCommandRequest>
    {
        public DeleteTempEstateVerificationCommandValidator()
        {
            RuleFor(x => x.Id).NotNull().WithMessage("Id gereklidir");
        }
    }
}
