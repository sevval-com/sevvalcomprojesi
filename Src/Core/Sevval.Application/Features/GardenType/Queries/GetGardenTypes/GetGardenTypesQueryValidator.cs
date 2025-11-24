using FluentValidation;

namespace Sevval.Application.Features.GardenType.Queries.GetGardenTypes;

public class GetGardenTypesQueryValidator : AbstractValidator<GetGardenTypesQueryRequest>
{
    public GetGardenTypesQueryValidator()
    {
        // No validation rules needed for this simple query
    }
}
