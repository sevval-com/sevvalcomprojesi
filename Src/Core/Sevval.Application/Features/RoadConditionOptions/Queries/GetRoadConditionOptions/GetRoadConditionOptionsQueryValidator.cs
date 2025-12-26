using FluentValidation;

namespace Sevval.Application.Features.RoadConditionOptions.Queries.GetRoadConditionOptions
{
    public class GetRoadConditionOptionsQueryValidator : AbstractValidator<GetRoadConditionOptionsQueryRequest>
    {
        public GetRoadConditionOptionsQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
