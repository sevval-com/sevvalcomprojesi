namespace Sevval.Application.Features.SalesRequest.Commands.CreateSalesRequest;

public class CreateSalesRequestCommandResponse
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; } = string.Empty;
    public int? RequestNumber { get; set; }
    public DateTime? CreatedDate { get; set; }
}
