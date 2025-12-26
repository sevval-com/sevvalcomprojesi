using System.Collections.Generic;

namespace Sevval.Application.Features.FieldStatus.Queries.GetFieldStatuses
{
    public class GetFieldStatusesQueryResponse
    {
        public List<FieldStatusDto> FieldStatuses { get; set; } = new List<FieldStatusDto>();
    }

    public class FieldStatusDto
    {
        public string Value { get; set; }
        public string DisplayName { get; set; }
        public string Description { get; set; }
    }
}
