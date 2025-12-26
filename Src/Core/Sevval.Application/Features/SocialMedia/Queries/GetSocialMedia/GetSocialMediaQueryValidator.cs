using FluentValidation;

namespace Sevval.Application.Features.SocialMedia.Queries.GetSocialMedia
{
    public class GetSocialMediaQueryValidator : AbstractValidator<GetSocialMediaQueryRequest>
    {
        public GetSocialMediaQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
