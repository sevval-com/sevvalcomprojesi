using MediatR;
using Sevval.Application.Features.Common;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sevval.Application.Features.User.Commands.UpdatePassword;

public class UpdatePasswordCommandRequest : IRequest<ApiResponse<UpdatePasswordCommandResponse>>
{
    public const string Route = "/api/v1/users/update-password";

    [NotMapped]
    [SwaggerIgnore]
    public string UserId { get; set; }
    public string NewPassword { get; set; }
    public string ConfirmNewPassword { get; set; }
    public string CurrentPassword { get; set; }
}
