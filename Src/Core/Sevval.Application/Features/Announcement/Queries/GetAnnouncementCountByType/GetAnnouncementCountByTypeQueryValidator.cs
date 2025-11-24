using FluentValidation;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByType
{
    public class GetAnnouncementCountByTypeQueryValidator : AbstractValidator<GetAnnouncementCountByTypeQueryRequest>
    {
        public GetAnnouncementCountByTypeQueryValidator()
        {
            RuleFor(x => x.Status)
                .MaximumLength(50)
                .WithMessage("Durum alanı en fazla 50 karakter olabilir.");

           
        }
    }
}
