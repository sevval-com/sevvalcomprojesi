using FluentValidation;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementsByUser
{
    public class GetAnnouncementsByUserQueryValidator : AbstractValidator<GetAnnouncementsByUserQueryRequest>
    {
        public GetAnnouncementsByUserQueryValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .WithMessage("E-posta adresi gereklidir.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta adresi giriniz.")
                .MaximumLength(255)
                .WithMessage("E-posta adresi 255 karakterden uzun olamaz.");

            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

            RuleFor(x => x.Size)
                .InclusiveBetween(1, 100)
                .WithMessage("Sayfa boyutu 1 ile 100 arasında olmalıdır.");

         
        }
    }
}
