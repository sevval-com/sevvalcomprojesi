using FluentValidation;

namespace Sevval.Application.Features.Announcement.Queries.SearchAnnouncements
{
    public class SearchAnnouncementsQueryValidator : AbstractValidator<SearchAnnouncementsQueryRequest>
    {
        public SearchAnnouncementsQueryValidator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0)
                .WithMessage("Sayfa numarası 0'dan büyük olmalıdır.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("Sayfa boyutu 1 ile 100 arasında olmalıdır.");

            RuleFor(x => x.MinPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinPrice.HasValue)
                .WithMessage("Minimum fiyat 0'dan küçük olamaz.");

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxPrice.HasValue)
                .WithMessage("Maksimum fiyat 0'dan küçük olamaz.");

            RuleFor(x => x.MaxPrice)
                .GreaterThanOrEqualTo(x => x.MinPrice)
                .When(x => x.MinPrice.HasValue && x.MaxPrice.HasValue)
                .WithMessage("Maksimum fiyat minimum fiyattan küçük olamaz.");

            RuleFor(x => x.MinArea)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MinArea.HasValue)
                .WithMessage("Minimum alan 0'dan küçük olamaz.");

            RuleFor(x => x.MaxArea)
                .GreaterThanOrEqualTo(0)
                .When(x => x.MaxArea.HasValue)
                .WithMessage("Maksimum alan 0'dan küçük olamaz.");

            RuleFor(x => x.MaxArea)
                .GreaterThanOrEqualTo(x => x.MinArea)
                .When(x => x.MinArea.HasValue && x.MaxArea.HasValue)
                .WithMessage("Maksimum alan minimum alandan küçük olamaz.");

            RuleFor(x => x.EndDate)
                .GreaterThanOrEqualTo(x => x.StartDate)
                .When(x => x.StartDate.HasValue && x.EndDate.HasValue)
                .WithMessage("Bitiş tarihi başlangıç tarihinden küçük olamaz.");

            RuleFor(x => x.SortBy)
                .Must(x => string.IsNullOrEmpty(x) || new[] { "Date", "Price", "Area", "Title" }.Contains(x))
                .WithMessage("Sıralama kriteri Date, Price, Area veya Title olmalıdır.");

            RuleFor(x => x.SortOrder)
                .Must(x => string.IsNullOrEmpty(x) || new[] { "ASC", "DESC" }.Contains(x))
                .WithMessage("Sıralama yönü ASC veya DESC olmalıdır.");

            RuleFor(x => x.Category)
                .Must(x => string.IsNullOrEmpty(x) || new[] { "Konut", "İş Yeri", "Arsa", "Bahçe", "Tarla", "Turistik Tesis" }.Contains(x))
                .WithMessage("Geçerli kategori seçiniz.");

            RuleFor(x => x.PropertyStatus)
                .Must(x => string.IsNullOrEmpty(x) || new[] { "Satılık", "Kiralık" }.Contains(x))
                .WithMessage("Mülk durumu Satılık veya Kiralık olmalıdır.");

            RuleFor(x => x.Status)
                .Must(x => string.IsNullOrEmpty(x) || new[] { "active", "passive", "pending" }.Contains(x))
                .WithMessage("İlan durumu active, passive veya pending olmalıdır.");

            RuleFor(x => x.Keyword)
                .MaximumLength(100)
                .When(x => !string.IsNullOrEmpty(x.Keyword))
                .WithMessage("Anahtar kelime 100 karakterden fazla olamaz.");
        }
    }
}
