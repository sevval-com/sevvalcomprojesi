using FluentValidation;

namespace Sevval.Application.Features.LandType.Queries.GetLandTypes;

public class GetLandTypesQueryValidator : AbstractValidator<GetLandTypesQueryRequest>
{
    public GetLandTypesQueryValidator()
    {
        // No validation rules needed for this simple query
    }
}
