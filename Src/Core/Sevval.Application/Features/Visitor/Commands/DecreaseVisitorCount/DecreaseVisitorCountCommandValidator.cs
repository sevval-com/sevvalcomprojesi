using FluentValidation;

namespace Sevval.Application.Features.Visitor.Commands.DecreaseVisitorCount
{
    public class DecreaseVisitorCountCommandValidator : AbstractValidator<DecreaseVisitorCountCommandRequest>
    {
        public DecreaseVisitorCountCommandValidator()
        {
            // Optional validation - IP address and user agent can be null
        }
    }
}
