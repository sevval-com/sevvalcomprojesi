using FluentValidation;

namespace Sevval.Application.Features.BusinessStatus.Queries.GetBusinessStatuses
{
    public class GetBusinessStatusesQueryValidator : AbstractValidator<GetBusinessStatusesQueryRequest>
    {
        public GetBusinessStatusesQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
