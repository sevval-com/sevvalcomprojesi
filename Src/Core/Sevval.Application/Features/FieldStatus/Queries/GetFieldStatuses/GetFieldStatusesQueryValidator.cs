using FluentValidation;

namespace Sevval.Application.Features.FieldStatus.Queries.GetFieldStatuses
{
    public class GetFieldStatusesQueryValidator : AbstractValidator<GetFieldStatusesQueryRequest>
    {
        public GetFieldStatusesQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
