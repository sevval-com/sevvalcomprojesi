using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.User.Commands.IndividualRegister;

public class IndividualRegisterCommandRequest : IRequest<ApiResponse<IndividualRegisterCommandResponse>>
{
    public const string Route = "/api/v1/individual-register";
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public string Password { get; set; }
    public string ConfirmPassword { get; set; }
    public string? Source { get; set; }
    public IFormFile? ProfilePicture { get; set; }
  
}
