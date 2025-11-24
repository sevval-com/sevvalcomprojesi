namespace Sevval.Application.Features.InvestmentRequest.Commands.CreateInvestmentRequest;

public class CreateInvestmentRequestCommandResponse
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; } = string.Empty;
    public DateTime? CreatedDate { get; set; }
}
