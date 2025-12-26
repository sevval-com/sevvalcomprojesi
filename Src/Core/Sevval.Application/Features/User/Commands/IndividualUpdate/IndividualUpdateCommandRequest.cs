using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.User.Commands.IndividualUpdate;

public class IndividualUpdateCommandRequest : IRequest<ApiResponse<IndividualUpdateCommandResponse>>
{
    public const string Route = "/api/v1/individual-users";
    public string Id { get; set; } // User ID to identify which user to update
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string? Password { get; set; } // Optional - only update if provided
    public string? ConfirmPassword { get; set; }
    public IFormFile? ProfilePicture { get; set; }
    public bool RemoveProfilePicture { get; set; } = false; // Flag to remove existing profile picture
}
