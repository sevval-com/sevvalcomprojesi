using Sevval.Application.Constants;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.Visitor.Commands.DecreaseVisitorCount;
using Sevval.Application.Features.Visitor.Commands.IncreaseVisitorCount;
using Sevval.Application.Features.Visitor.Queries.GetActiveVisitorCount;
using Sevval.Application.Features.Visitor.Queries.GetTotalVisitorCount;

namespace sevvalemlak.csproj.ClientServices.VisitoryServices
{
    public class VisitoryClientService : IVisitoryClientService
    {
        private readonly HttpClient _httpClient;
        public VisitoryClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ApiResponse<GetActiveVisitorCountQueryResponse>> GetActiveVisitorCount(GetActiveVisitorCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetActiveVisitorCountQueryRequest.Route, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponse<GetActiveVisitorCountQueryResponse>>(cancellationToken: cancellationToken);
        }


        public async Task<ApiResponse<GetTotalVisitorCountQueryResponse>> GetTotalVisitorCount(GetTotalVisitorCountQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.GetAsync(GeneralConstants.BaseClientUrl + GetTotalVisitorCountQueryRequest.Route, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponse<GetTotalVisitorCountQueryResponse>>(cancellationToken: cancellationToken);
        }


        public async Task<ApiResponse<IncreaseVisitorCountCommandResponse>> IncreaseVisitorCount(IncreaseVisitorCountCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(GeneralConstants.BaseClientUrl + IncreaseVisitorCountCommandRequest.Route, request, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponse<IncreaseVisitorCountCommandResponse>>(cancellationToken: cancellationToken);
        }


        public async Task<ApiResponse<DecreaseVisitorCountCommandResponse>> DecreaseVisitorCount(DecreaseVisitorCountCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(GeneralConstants.BaseClientUrl + DecreaseVisitorCountCommandRequest.Route,request, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponse<DecreaseVisitorCountCommandResponse>>(cancellationToken: cancellationToken);
        }
    }
}
