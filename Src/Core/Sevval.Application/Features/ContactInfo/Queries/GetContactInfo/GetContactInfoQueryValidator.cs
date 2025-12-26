using FluentValidation;

namespace Sevval.Application.Features.ContactInfo.Queries.GetContactInfo
{
    public class GetContactInfoQueryValidator : AbstractValidator<GetContactInfoQueryRequest>
    {
        public GetContactInfoQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
