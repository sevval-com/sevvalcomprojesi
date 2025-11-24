using FluentValidation;

namespace Sevval.Application.Features.BusinessType.Queries.GetBusinessTypes
{
    public class GetBusinessTypesQueryValidator : AbstractValidator<GetBusinessTypesQueryRequest>
    {
        public GetBusinessTypesQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
