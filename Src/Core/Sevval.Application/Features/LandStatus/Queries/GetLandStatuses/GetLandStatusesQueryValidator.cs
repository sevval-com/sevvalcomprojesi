using FluentValidation;

namespace Sevval.Application.Features.LandStatus.Queries.GetLandStatuses;

public class GetLandStatusesQueryValidator : AbstractValidator<GetLandStatusesQueryRequest>
{
    public GetLandStatusesQueryValidator()
    {
        // No validation rules needed for this simple query
    }
}
