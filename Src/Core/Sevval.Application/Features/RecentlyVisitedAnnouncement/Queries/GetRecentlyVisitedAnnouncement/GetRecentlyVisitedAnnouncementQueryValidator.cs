using FluentValidation;

namespace Sevval.Application.Features.RecentlyVisitedAnnouncement.Queries.GetRecentlyVisitedAnnouncement
{
    public class GetRecentlyVisitedAnnouncementQueryValidator : AbstractValidator<GetRecentlyVisitedAnnouncementQueryRequest>
    {
        public GetRecentlyVisitedAnnouncementQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("Kullanıcı ID'si gereklidir.");

         
        }
    }
}
