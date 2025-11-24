namespace Sevval.Application.Features.PropertyStatus.Queries.GetPropertyStatuses;

public class GetPropertyStatusesQueryResponse
{
    public List<PropertyStatusDto> PropertyStatuses { get; set; } = new();
    public string Message { get; set; } = string.Empty;
}

public class PropertyStatusDto
{
    public string Value { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}
