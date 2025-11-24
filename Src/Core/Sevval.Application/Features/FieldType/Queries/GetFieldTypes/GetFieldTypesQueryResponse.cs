using Sevval.Application.DTOs;

namespace Sevval.Application.Features.FieldType.Queries.GetFieldTypes;

public class GetFieldTypesQueryResponse
{
    public List<FieldTypeDTO> FieldTypes { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class FieldTypeDTO
{
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
