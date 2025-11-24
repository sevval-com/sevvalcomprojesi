using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.RoomOptions.Queries.GetRoomOptions;

public class GetRoomOptionsQueryRequest : IRequest<ApiResponse<GetRoomOptionsQueryResponse>>
{
    public const string Route = "/api/v1/room-options";
}
