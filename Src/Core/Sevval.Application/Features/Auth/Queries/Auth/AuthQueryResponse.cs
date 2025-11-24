namespace Sevval.Application.Features.Auth.Queries.Auth
{
    public class AuthQueryResponse
    {
        public string AccessToken { get; set; }

        public DateTime AccessTokenExpiration { get; set; }

        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
