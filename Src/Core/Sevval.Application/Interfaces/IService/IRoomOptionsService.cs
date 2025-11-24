using Sevval.Application.Features.RoomOptions.Queries.GetRoomOptions;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService;

public interface IRoomOptionsService
{
    Task<ApiResponse<GetRoomOptionsQueryResponse>> GetRoomOptionsAsync(CancellationToken cancellationToken = default);
}
