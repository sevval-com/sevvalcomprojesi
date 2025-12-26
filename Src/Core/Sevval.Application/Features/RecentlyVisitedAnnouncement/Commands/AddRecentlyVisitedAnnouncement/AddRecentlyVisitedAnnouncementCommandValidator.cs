using FluentValidation;

namespace Sevval.Application.Features.RecentlyVisitedAnnouncement.Commands.AddRecentlyVisitedAnnouncement
{
    public class AddRecentlyVisitedAnnouncementCommandValidator : AbstractValidator<AddRecentlyVisitedAnnouncementCommandRequest>
    {
        public AddRecentlyVisitedAnnouncementCommandValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("Kullanıcı ID'si gereklidir.");

            RuleFor(x => x.AnnouncementId)
                .GreaterThan(0)
                .WithMessage("İlan ID'si geçerli bir değer olmalıdır.");

           
        }
    }
}
