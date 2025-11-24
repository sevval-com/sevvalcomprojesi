using Sevval.Application.DTOs.LandType;

namespace Sevval.Application.Features.LandType.Queries.GetLandTypes;

public class GetLandTypesQueryResponse
{
    public List<LandTypeDTO> LandTypes { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
