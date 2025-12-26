using FluentValidation;

namespace Sevval.Application.Features.BalconyOptions.Queries.GetBalconyOptions
{
    public class GetBalconyOptionsQueryValidator : AbstractValidator<GetBalconyOptionsQueryRequest>
    {
        public GetBalconyOptionsQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
