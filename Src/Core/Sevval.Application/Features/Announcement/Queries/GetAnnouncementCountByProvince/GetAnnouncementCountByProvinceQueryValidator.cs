using FluentValidation;

namespace Sevval.Application.Features.Announcement.Queries.GetAnnouncementCountByProvince
{
    public class GetAnnouncementCountByProvinceQueryValidator : AbstractValidator<GetAnnouncementCountByProvinceQueryRequest>
    {
        public GetAnnouncementCountByProvinceQueryValidator()
        {
            RuleFor(x => x.Status)
                .MaximumLength(50)
                .WithMessage("Durum alanı en fazla 50 karakter olabilir.");

           
        }
    }
}
