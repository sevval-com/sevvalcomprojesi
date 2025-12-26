using FluentValidation;

namespace Sevval.Application.Features.BathroomOptions.Queries.GetBathroomOptions
{
    public class GetBathroomOptionsQueryValidator : AbstractValidator<GetBathroomOptionsQueryRequest>
    {
        public GetBathroomOptionsQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
