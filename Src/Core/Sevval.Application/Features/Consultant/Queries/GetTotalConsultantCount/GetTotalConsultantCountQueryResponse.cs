namespace Sevval.Application.Features.Consultant.Queries.GetTotalConsultantCount;

public class GetTotalConsultantCountQueryResponse
{
    public int TotalCount { get; set; }
    public string? Status { get; set; }
    public string? CompanyName { get; set; }
    public string Message { get; set; } = string.Empty;
}
