using Sevval.Application.Constants;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.InvestmentRequest.Commands.CreateInvestmentRequest;
using Sevval.Application.Features.SalesRequest.Commands.CreateSalesRequest;

namespace sevvalemlak.csproj.ClientServices.SalesRequestService
{
    public class SalesRequestClientService : ISalesRequestClientService
    {
        private readonly HttpClient _httpClient;
        public SalesRequestClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<CreateSalesRequestCommandResponse>> CreateSalesRequestAsync(CreateSalesRequestCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(GeneralConstants.BaseClientUrl + CreateSalesRequestCommandRequest.Route, request, cancellationToken);

            return await response.Content.ReadFromJsonAsync<ApiResponse<CreateSalesRequestCommandResponse>>(cancellationToken: cancellationToken);


        }
        public async Task<ApiResponse<CreateInvestmentRequestCommandResponse>> CreateInvestmentRequestAsync(CreateInvestmentRequestCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(GeneralConstants.BaseClientUrl + CreateInvestmentRequestCommandRequest.Route, request, cancellationToken);

            return await response.Content.ReadFromJsonAsync<ApiResponse<CreateInvestmentRequestCommandResponse>>(cancellationToken: cancellationToken);


        }


    }
}
