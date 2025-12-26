using FluentValidation;

namespace Sevval.Application.Features.PropertyType.Queries.GetPropertyTypes;

public class GetPropertyTypesQueryValidator : AbstractValidator<GetPropertyTypesQueryRequest>
{
    public GetPropertyTypesQueryValidator()
    {
        // No validation rules needed for this simple query
    }
}
