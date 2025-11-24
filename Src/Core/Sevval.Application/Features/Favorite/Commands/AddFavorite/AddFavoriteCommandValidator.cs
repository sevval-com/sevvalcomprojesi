using FluentValidation;

namespace Sevval.Application.Features.Favorite.Commands.AddFavorite
{
    public class AddFavoriteCommandValidator : AbstractValidator<AddFavoriteCommandRequest>
    {
        public AddFavoriteCommandValidator()
        {
            RuleFor(x => x.AnnouncementId)
                .GreaterThan(0)
                .WithMessage("İlan ID'si geçerli olmalıdır.");

            RuleFor(x => x.UserEmail)
                .NotEmpty()
                .WithMessage("Kullanıcı e-posta adresi gereklidir.")
                .EmailAddress()
                .WithMessage("Geçerli bir e-posta adresi giriniz.");

            RuleFor(x => x.IpAddress)
                .NotEmpty()
                .WithMessage("IP adresi gereklidir.");
        }
    }
}
