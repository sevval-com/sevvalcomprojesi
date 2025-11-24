using FluentValidation;

namespace Sevval.Application.Features.SlopeOptions.Queries.GetSlopeOptions
{
    public class GetSlopeOptionsQueryValidator : AbstractValidator<GetSlopeOptionsQueryRequest>
    {
        public GetSlopeOptionsQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
