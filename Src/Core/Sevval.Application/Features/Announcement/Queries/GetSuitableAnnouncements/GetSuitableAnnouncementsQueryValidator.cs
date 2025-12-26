using FluentValidation;

namespace Sevval.Application.Features.Announcement.Queries.GetSuitableAnnouncements
{
    public class GetSuitableAnnouncementsQueryValidator : AbstractValidator<GetSuitableAnnouncementsQueryRequest>
    {
        public GetSuitableAnnouncementsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

         

           
        }
    }
}
