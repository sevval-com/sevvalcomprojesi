using FluentValidation;

namespace Sevval.Application.Features.Visitor.Commands.IncreaseVisitorCount
{
    public class IncreaseVisitorCountCommandValidator : AbstractValidator<IncreaseVisitorCountCommandRequest>
    {
        public IncreaseVisitorCountCommandValidator()
        {
            // Optional validation - IP address and user agent can be null
        }
    }
}
