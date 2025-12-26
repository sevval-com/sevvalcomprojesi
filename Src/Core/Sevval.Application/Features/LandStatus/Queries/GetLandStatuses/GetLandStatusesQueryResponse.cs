using Sevval.Application.DTOs.LandStatus;

namespace Sevval.Application.Features.LandStatus.Queries.GetLandStatuses;

public class GetLandStatusesQueryResponse
{
    public List<LandStatusDTO> LandStatuses { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
