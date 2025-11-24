using FluentValidation;

namespace Sevval.Application.Features.User.Commands.SendNewCode;


public class SendNewCodeCommandValidator : AbstractValidator<SendNewCodeCommandRequest>
{
    public SendNewCodeCommandValidator()
    {

        RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta gereklidir").EmailAddress().WithMessage("Geçersiz e-posta adresi");
    }
}
