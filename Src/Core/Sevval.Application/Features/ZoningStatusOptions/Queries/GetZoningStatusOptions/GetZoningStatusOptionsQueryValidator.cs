using FluentValidation;

namespace Sevval.Application.Features.ZoningStatusOptions.Queries.GetZoningStatusOptions
{
    public class GetZoningStatusOptionsQueryValidator : AbstractValidator<GetZoningStatusOptionsQueryRequest>
    {
        public GetZoningStatusOptionsQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
