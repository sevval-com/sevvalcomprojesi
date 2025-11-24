using FluentValidation;

namespace Sevval.Application.Features.FieldType.Queries.GetFieldTypes;

public class GetFieldTypesQueryValidator : AbstractValidator<GetFieldTypesQueryRequest>
{
    public GetFieldTypesQueryValidator()
    {
        // No validation rules needed for this simple query
    }
}
