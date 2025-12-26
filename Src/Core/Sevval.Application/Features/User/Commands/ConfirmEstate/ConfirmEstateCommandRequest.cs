using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.User.Commands.ConfirmEstate;

public class ConfirmEstateCommandRequest : IRequest<ApiResponse<ConfirmEstateCommandResponse>>
{
    public const string Route = "/api/v1/confirm-estate";
    
    public string Token { get; set; }
    public string Email { get; set; }
}
