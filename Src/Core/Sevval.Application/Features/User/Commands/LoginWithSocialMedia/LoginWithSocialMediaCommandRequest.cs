using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.User.Commands.LoginWithSocialMedia;

public class LoginWithSocialMediaCommandRequest : IRequest<ApiResponse<LoginWithSocialMediaCommandResponse>>
{
    public const string Route = "/api/v1/auth-social";
    
    public string Provider { get; set; }
    public string? Token { get; set; }
    public string SocialId { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public string? PhotoUrl { get; set; }
    public string UserType { get; set; }
}
