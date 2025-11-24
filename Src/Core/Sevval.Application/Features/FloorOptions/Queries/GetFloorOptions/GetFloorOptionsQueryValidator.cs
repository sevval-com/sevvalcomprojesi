using FluentValidation;

namespace Sevval.Application.Features.FloorOptions.Queries.GetFloorOptions
{
    public class GetFloorOptionsQueryValidator : AbstractValidator<GetFloorOptionsQueryRequest>
    {
        public GetFloorOptionsQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
