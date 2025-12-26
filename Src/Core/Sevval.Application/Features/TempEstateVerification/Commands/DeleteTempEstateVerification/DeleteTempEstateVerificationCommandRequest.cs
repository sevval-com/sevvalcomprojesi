using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.TempEstateVerification.Commands.DeleteTempEstateVerification;

public class DeleteTempEstateVerificationCommandRequest : IRequest<ApiResponse<DeleteTempEstateVerificationCommandResponse>>
{
    public const string Route = "/api/v1/temp-estate-verification";
    
    public int Id { get; set; }
}
