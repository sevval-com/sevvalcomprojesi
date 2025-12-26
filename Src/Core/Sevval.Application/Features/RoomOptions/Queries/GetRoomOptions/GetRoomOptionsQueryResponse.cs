using Sevval.Application.DTOs.RoomOptions;

namespace Sevval.Application.Features.RoomOptions.Queries.GetRoomOptions;

public class GetRoomOptionsQueryResponse
{
    public List<RoomOptionDTO> RoomOptions { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
