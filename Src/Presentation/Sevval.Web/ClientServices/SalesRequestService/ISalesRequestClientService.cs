using Sevval.Application.Features.Common;
using Sevval.Application.Features.InvestmentRequest.Commands.CreateInvestmentRequest;
using Sevval.Application.Features.SalesRequest.Commands.CreateSalesRequest;

namespace sevvalemlak.csproj.ClientServices.SalesRequestService
{
    public interface ISalesRequestClientService
    {
        Task<ApiResponse<CreateInvestmentRequestCommandResponse>> CreateInvestmentRequestAsync(CreateInvestmentRequestCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<CreateSalesRequestCommandResponse>> CreateSalesRequestAsync(CreateSalesRequestCommandRequest request, CancellationToken cancellationToken);
    }
}