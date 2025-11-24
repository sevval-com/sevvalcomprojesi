using FluentValidation;

namespace Sevval.Application.Features.Visitor.Queries.GetTotalVisitorCount
{
    public class GetTotalVisitorCountQueryValidator : AbstractValidator<GetTotalVisitorCountQueryRequest>
    {
        public GetTotalVisitorCountQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
