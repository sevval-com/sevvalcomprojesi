using FluentValidation;

namespace Sevval.Application.Features.HeatingSystemOptions.Queries.GetHeatingSystemOptions
{
    public class GetHeatingSystemOptionsQueryValidator : AbstractValidator<GetHeatingSystemOptionsQueryRequest>
    {
        public GetHeatingSystemOptionsQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
