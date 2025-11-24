using FluentValidation;

namespace Sevval.Application.Features.WhatsApp.Queries.GetWhatsAppSettings
{
    public class GetWhatsAppSettingsQueryValidator : AbstractValidator<GetWhatsAppSettingsQueryRequest>
    {
        public GetWhatsAppSettingsQueryValidator()
        {
            // No validation needed for this simple query
        }
    }
}
