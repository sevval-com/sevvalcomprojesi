using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.TempEstateVerification.Queries.GetTempEstateVerification;

public class GetTempEstateVerificationQueryHandler : BaseHandler, IRequestHandler<GetTempEstateVerificationQueryRequest, ApiResponse<GetTempEstateVerificationQueryResponse>>
{
    private readonly ITempEstateVerificationService _tempEstateVerificationService;

    public GetTempEstateVerificationQueryHandler(IHttpContextAccessor httpContextAccessor, ITempEstateVerificationService tempEstateVerificationService) : base(httpContextAccessor)
    {
        _tempEstateVerificationService = tempEstateVerificationService;
    }

    public async Task<ApiResponse<GetTempEstateVerificationQueryResponse>> Handle(GetTempEstateVerificationQueryRequest request, CancellationToken cancellationToken)
    {
        var result = await _tempEstateVerificationService.GetTempEstateVerification(request, cancellationToken);

        var response = new ApiResponse<GetTempEstateVerificationQueryResponse>
        {
            IsSuccessfull = result.IsSuccessfull,
            Message = result.Message
        };

        if (result.IsSuccessfull && result.Data != null)
        {
            response.Data = new GetTempEstateVerificationQueryResponse
            {
                Email = result.Data.Email,
                Code = result.Data.Code,
                Expiration = result.Data.Expiration,
                IsSuccessfull = true,
                Message = "Verification found"
            };
        }

        return response;
    }
}
