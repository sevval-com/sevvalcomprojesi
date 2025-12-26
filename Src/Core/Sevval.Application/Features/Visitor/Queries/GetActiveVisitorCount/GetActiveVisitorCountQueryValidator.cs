using FluentValidation;

namespace Sevval.Application.Features.Visitor.Queries.GetActiveVisitorCount
{
    public class GetActiveVisitorCountQueryValidator : AbstractValidator<GetActiveVisitorCountQueryRequest>
    {
        public GetActiveVisitorCountQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
