using FluentValidation;

namespace Sevval.Application.Features.FacilityType.Queries.GetFacilityTypes;

public class GetFacilityTypesQueryValidator : AbstractValidator<GetFacilityTypesQueryRequest>
{
    public GetFacilityTypesQueryValidator()
    {
        // No validation rules needed for this simple query
    }
}
