using FluentValidation;

namespace Sevval.Application.Features.GardenStatus.Queries.GetGardenStatuses;

public class GetGardenStatusesQueryValidator : AbstractValidator<GetGardenStatusesQueryRequest>
{
    public GetGardenStatusesQueryValidator()
    {
        // No validation rules needed for this simple request
    }
}
