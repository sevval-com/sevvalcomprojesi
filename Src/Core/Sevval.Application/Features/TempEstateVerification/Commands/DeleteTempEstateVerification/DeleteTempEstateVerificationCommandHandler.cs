using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.TempEstateVerification.Commands.DeleteTempEstateVerification;

public class DeleteTempEstateVerificationCommandHandler : BaseHandler, IRequestHandler<DeleteTempEstateVerificationCommandRequest, ApiResponse<DeleteTempEstateVerificationCommandResponse>>
{
    private readonly ITempEstateVerificationService _tempEstateVerificationService;

    public DeleteTempEstateVerificationCommandHandler(IHttpContextAccessor httpContextAccessor, ITempEstateVerificationService tempEstateVerificationService) : base(httpContextAccessor)
    {
        _tempEstateVerificationService = tempEstateVerificationService;
    }

    public async Task<ApiResponse<DeleteTempEstateVerificationCommandResponse>> Handle(DeleteTempEstateVerificationCommandRequest request, CancellationToken cancellationToken)
    {
        var result = await _tempEstateVerificationService.DeleteTempEstateVerification(request, cancellationToken);

        return new ApiResponse<DeleteTempEstateVerificationCommandResponse>
        {
            IsSuccessfull = result.IsSuccessfull,
            Message = result.Message,
            Data = new DeleteTempEstateVerificationCommandResponse
            {
                IsSuccessfull = result.IsSuccessfull,
                Message = result.Message
            }
        };
    }
}
