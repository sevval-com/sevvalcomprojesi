using Sevval.Application.Features.Common;
using Sevval.Application.Features.InvestmentRequest.Commands.CreateInvestmentRequest;

namespace Sevval.Application.Abstractions.Services;

public interface IInvestmentRequestService
{
    Task<ApiResponse<CreateInvestmentRequestCommandResponse>> CreateInvestmentRequestAsync(CreateInvestmentRequestCommandRequest request, CancellationToken cancellationToken);
}
