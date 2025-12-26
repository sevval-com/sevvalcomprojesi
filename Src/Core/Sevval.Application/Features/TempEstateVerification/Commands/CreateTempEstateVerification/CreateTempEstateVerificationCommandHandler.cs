using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.TempEstateVerification.Commands.CreateTempEstateVerification;

public class CreateTempEstateVerificationCommandHandler : BaseHandler, IRequestHandler<CreateTempEstateVerificationCommandRequest, ApiResponse<CreateTempEstateVerificationCommandResponse>>
{
    private readonly ITempEstateVerificationService _tempEstateVerificationService;

    public CreateTempEstateVerificationCommandHandler(IHttpContextAccessor httpContextAccessor, ITempEstateVerificationService tempEstateVerificationService) : base(httpContextAccessor)
    {
        _tempEstateVerificationService = tempEstateVerificationService;
    }

    public async Task<ApiResponse<CreateTempEstateVerificationCommandResponse>> Handle(CreateTempEstateVerificationCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _tempEstateVerificationService.CreateTempEstateVerification(request, cancellationToken);
                

        return response;
    }
}
