using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.User.Commands.RejectEstate;

public class RejectEstateCommandRequest : IRequest<ApiResponse<RejectEstateCommandResponse>>
{
    public const string Route = "/api/v1/reject-estate";
    
    public string Token { get; set; }
    public string Email { get; set; }
}
