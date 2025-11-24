using FluentValidation;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementDetails
{
    public class GetAnnouncementDetailsQueryValidator : AbstractValidator<GetAnnouncementDetailsQueryRequest>
    {
        public GetAnnouncementDetailsQueryValidator()
        {
            RuleFor(x => x.Id)
                .GreaterThan(0)
                .WithMessage("İlan ID'si 0'dan büyük olmalıdır.");
        }
    }
}
