using MediatR;
using Sevval.Application.Abstractions.Services;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.InvestmentRequest.Commands.CreateInvestmentRequest;

public class CreateInvestmentRequestCommandHandler : IRequestHandler<CreateInvestmentRequestCommandRequest, ApiResponse<CreateInvestmentRequestCommandResponse>>
{
    private readonly IInvestmentRequestService _investmentRequestService;

    public CreateInvestmentRequestCommandHandler(IInvestmentRequestService investmentRequestService)
    {
        _investmentRequestService = investmentRequestService;
    }

    public async Task<ApiResponse<CreateInvestmentRequestCommandResponse>> Handle(CreateInvestmentRequestCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _investmentRequestService.CreateInvestmentRequestAsync(request, cancellationToken);
        return response;
    }
}
