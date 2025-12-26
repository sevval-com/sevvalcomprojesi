using FluentValidation;

namespace Sevval.Application.Features.BuildingAge.Queries.GetBuildingAges;

public class GetBuildingAgesQueryValidator : AbstractValidator<GetBuildingAgesQueryRequest>
{
    public GetBuildingAgesQueryValidator()
    {
        // No validation rules needed for this simple query
    }
}
