using FluentValidation;

namespace Sevval.Application.Features.Announcement.Queries.GetTodaysAnnouncements
{
    public class GetTodaysAnnouncementsQueryValidator : AbstractValidator<GetTodaysAnnouncementsQueryRequest>
    {
        public GetTodaysAnnouncementsQueryValidator()
        {
            RuleFor(x => x.Status)
                .MaximumLength(50)
                .WithMessage("Durum alanı en fazla 50 karakter olabilir.");
        }
    }
}
