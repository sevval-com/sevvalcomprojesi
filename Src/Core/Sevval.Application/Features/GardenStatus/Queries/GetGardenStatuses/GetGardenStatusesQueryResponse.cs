using Sevval.Application.DTOs.GardenStatus;

namespace Sevval.Application.Features.GardenStatus.Queries.GetGardenStatuses;

public class GetGardenStatusesQueryResponse
{
    public List<GardenStatusDTO> GardenStatuses { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
