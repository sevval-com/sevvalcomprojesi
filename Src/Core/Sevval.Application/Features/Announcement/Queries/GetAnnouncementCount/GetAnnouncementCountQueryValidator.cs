using FluentValidation;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementCount
{
    public class GetAnnouncementCountQueryValidator : AbstractValidator<GetAnnouncementCountQueryRequest>
    {
        public GetAnnouncementCountQueryValidator()
        {
            RuleFor(x => x.Status)
                .MaximumLength(50)
                .WithMessage("Durum en fazla 50 karakter olabilir.");

        }
    }
}
