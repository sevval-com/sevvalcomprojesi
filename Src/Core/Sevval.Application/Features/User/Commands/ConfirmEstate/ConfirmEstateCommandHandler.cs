using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.TempEstateVerification.Commands.DeleteTempEstateVerification;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.ConfirmEstate;

public class ConfirmEstateCommandHandler : BaseHandler, IRequestHandler<ConfirmEstateCommandRequest, ApiResponse<ConfirmEstateCommandResponse>>
{
    private readonly IUserService _userService;
    private readonly ITempEstateVerificationService _tempEstateVerificationService;
    private readonly IEMailService _emailService;

    public ConfirmEstateCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, ITempEstateVerificationService tempEstateVerificationService, IEMailService emailService) : base(httpContextAccessor)
    {
        _userService = userService;
        _tempEstateVerificationService = tempEstateVerificationService;
        _emailService = emailService;
    }

    public async Task<ApiResponse<ConfirmEstateCommandResponse>> Handle(ConfirmEstateCommandRequest request, CancellationToken cancellationToken)
    {


        var tempEstate = await _tempEstateVerificationService.GetTempEstateVerification(new TempEstateVerification.Queries.GetTempEstateVerification.GetTempEstateVerificationQueryRequest
        {
            Email = request.Email,
            Code = request.Token

        }, cancellationToken);

        if (!tempEstate.IsSuccessfull || tempEstate.Data == null)
        {
            return new ApiResponse<ConfirmEstateCommandResponse>
            {
                IsSuccessfull = false,
                Message = "Geçersiz veya süresi dolmuþ doðrulama kodu."
            };
        }

        var response = await _userService.ConfirmEstate(request, cancellationToken);

        if (response.IsSuccessfull)
        {
            await _emailService.SendConfirmeInformationMailToEstate(request.Email);

            await _tempEstateVerificationService.DeleteTempEstateVerification(new DeleteTempEstateVerificationCommandRequest
            {
                Id = tempEstate.Data.Id

            }, cancellationToken);
        }

        return response;
    }
}
