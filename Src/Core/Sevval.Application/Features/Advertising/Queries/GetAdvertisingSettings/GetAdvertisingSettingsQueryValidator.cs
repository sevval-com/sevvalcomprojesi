using FluentValidation;

namespace Sevval.Application.Features.Advertising.Queries.GetAdvertisingSettings
{
    public class GetAdvertisingSettingsQueryValidator : AbstractValidator<GetAdvertisingSettingsQueryRequest>
    {
        public GetAdvertisingSettingsQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
