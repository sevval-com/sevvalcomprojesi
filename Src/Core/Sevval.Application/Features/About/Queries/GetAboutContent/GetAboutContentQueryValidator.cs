using FluentValidation;

namespace Sevval.Application.Features.About.Queries.GetAboutContent
{
    public class GetAboutContentQueryValidator : AbstractValidator<GetAboutContentQueryRequest>
    {
        public GetAboutContentQueryValidator()
        {
            // No validation needed for this simple query
        }
    }
}
