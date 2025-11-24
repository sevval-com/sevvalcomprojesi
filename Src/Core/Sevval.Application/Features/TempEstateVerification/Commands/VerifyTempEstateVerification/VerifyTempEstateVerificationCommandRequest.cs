using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.TempEstateVerification.Commands.VerifyTempEstateVerification;

public class VerifyTempEstateVerificationCommandRequest : IRequest<ApiResponse<VerifyTempEstateVerificationCommandResponse>>
{
    public const string Route = "/api/v1/temp-estate-verification/verify";
    
    public string Email { get; set; }
    public string Code { get; set; }
}
