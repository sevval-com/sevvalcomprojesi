using FluentValidation;

namespace Sevval.Application.Features.RoomOptions.Queries.GetRoomOptions;

public class GetRoomOptionsQueryValidator : AbstractValidator<GetRoomOptionsQueryRequest>
{
    public GetRoomOptionsQueryValidator()
    {
        // No validation rules needed for this simple query
    }
}
