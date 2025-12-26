namespace Sevval.Application.Features.User.Commands.LoginWithSocialMedia;

public class LoginWithSocialMediaCommandResponse
{
    public bool IsSuccessful { get; set; }
    public string Message { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiration { get; set; }
    public DateTime RefreshTokenExpiration { get; set; }
}
