using FluentValidation;

namespace Sevval.Application.Features.Announcement.Queries.GetCompanyAnnouncementCountByProvince
{
    public class GetCompanyAnnouncementCountByProvinceQueryValidator : AbstractValidator<GetCompanyAnnouncementCountByProvinceQueryRequest>
    {
        public GetCompanyAnnouncementCountByProvinceQueryValidator()
        {
           

            RuleFor(x => x.Status)
                .Must(status => string.IsNullOrEmpty(status) || 
                               new[] { "active", "passive" }.Contains(status.ToLower()))
                .WithMessage("Geçerli durum değerleri: active, passive, pending");
        }
    }
}
