using FluentValidation;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByCompany
{
    public class GetAnnouncementsByCompanyQueryValidator : AbstractValidator<GetAnnouncementsByCompanyQueryRequest>
    {
        public GetAnnouncementsByCompanyQueryValidator()
        {
            RuleFor(x => x.UserId)
                .NotEmpty()
                .WithMessage("Firma adı gereklidir.")
                .MaximumLength(200)
                .WithMessage("Firma adı en fazla 200 karakter olabilir.");

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Sayfa numarası 1'den büyük olmalıdır.");

            RuleFor(x => x.Size)
                .InclusiveBetween(1, 1000)
                .WithMessage("Sayfa boyutu 1 ile 1000 arasında olmalıdır.");
           
        }
    }
}
