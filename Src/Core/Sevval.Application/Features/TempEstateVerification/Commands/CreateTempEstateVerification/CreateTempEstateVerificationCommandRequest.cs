using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.TempEstateVerification.Commands.CreateTempEstateVerification;

public class CreateTempEstateVerificationCommandRequest : IRequest<ApiResponse<CreateTempEstateVerificationCommandResponse>>
{
    public const string Route = "/api/v1/temp-estate-verification";
    
    public string Email { get; set; }
}
