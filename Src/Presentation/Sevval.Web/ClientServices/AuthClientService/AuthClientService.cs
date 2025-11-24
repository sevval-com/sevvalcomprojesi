using Sevval.Application.Constants;
using Sevval.Application.Features.Auth.Queries.Auth;
using Sevval.Application.Features.Common;

namespace sevvalemlak.csproj.ClientServices.AuthClientService
{
    public class AuthClientService : IAuthClientService
    {
        private readonly HttpClient _httpClient;

        public AuthClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
             
        }

        public async Task<ApiResponse<AuthQueryResponse>> Auth(AuthQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(GeneralConstants.BaseClientUrl + AuthQueryRequest.Route, request, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponse<AuthQueryResponse>>(cancellationToken: cancellationToken);
        }
    }
}
