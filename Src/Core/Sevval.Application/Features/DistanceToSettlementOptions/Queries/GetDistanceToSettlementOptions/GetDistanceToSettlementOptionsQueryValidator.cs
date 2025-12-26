using FluentValidation;

namespace Sevval.Application.Features.DistanceToSettlementOptions.Queries.GetDistanceToSettlementOptions
{
    public class GetDistanceToSettlementOptionsQueryValidator : AbstractValidator<GetDistanceToSettlementOptionsQueryRequest>
    {
        public GetDistanceToSettlementOptionsQueryValidator()
        {
            // No validation rules needed for this simple query
        }
    }
}
