using FluentValidation;

namespace Sevval.Application.Features.FacilityStatus.Queries.GetFacilityStatuses;

public class GetFacilityStatusesQueryValidator : AbstractValidator<GetFacilityStatusesQueryRequest>
{
    public GetFacilityStatusesQueryValidator()
    {
        // No validation rules needed for this simple request
    }
}
