using Sevval.Application.DTOs.PropertyType;

namespace Sevval.Application.Features.PropertyType.Queries.GetPropertyTypes;

public class GetPropertyTypesQueryResponse
{
    public List<PropertyTypeDTO> PropertyTypes { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}
