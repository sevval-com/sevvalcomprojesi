using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.TempEstateVerification.Queries.GetTempEstateVerification;

public class GetTempEstateVerificationQueryRequest : IRequest<ApiResponse<GetTempEstateVerificationQueryResponse>>
{
    public const string Route = "/api/v1/temp-estate-verification/{email}";
    
    public string Email { get; set; }
    public string Code { get; set; }
}
