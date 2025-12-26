using Sevval.Application.DTOs.BuildingAge;

namespace Sevval.Application.Features.BuildingAge.Queries.GetBuildingAges;

public class GetBuildingAgesQueryResponse
{
    public List<BuildingAgeDTO> BuildingAges { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
