using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.TempEstateVerification.Commands.VerifyTempEstateVerification;

public class VerifyTempEstateVerificationCommandHandler : BaseHandler, IRequestHandler<VerifyTempEstateVerificationCommandRequest, ApiResponse<VerifyTempEstateVerificationCommandResponse>>
{
    private readonly ITempEstateVerificationService _tempEstateVerificationService;

    public VerifyTempEstateVerificationCommandHandler(IHttpContextAccessor httpContextAccessor, ITempEstateVerificationService tempEstateVerificationService) : base(httpContextAccessor)
    {
        _tempEstateVerificationService = tempEstateVerificationService;
    }

    public async Task<ApiResponse<VerifyTempEstateVerificationCommandResponse>> Handle(VerifyTempEstateVerificationCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _tempEstateVerificationService.VerifyTempEstateVerification(request, cancellationToken);

        return response;
    }
}
