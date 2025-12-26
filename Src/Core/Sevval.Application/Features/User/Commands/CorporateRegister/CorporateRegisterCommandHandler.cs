using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Constants;
using Sevval.Application.Dtos.Email;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.User.Commands.CorporateRegister;


public class CorporateRegisterCommandHandler : BaseHandler, IRequestHandler<CorporateRegisterCommandRequest, ApiResponse<CorporateRegisterCommandResponse>>
{
    private readonly IUserService _userService;
    private readonly ITempEstateVerificationService _tempEstateVerificationService;
    private readonly IEMailService _emailService;

    public CorporateRegisterCommandHandler(IHttpContextAccessor httpContextAccessor, IUserService userService, ITempEstateVerificationService tempEstateVerificationService, IEMailService emailService) : base(httpContextAccessor)
    {

        _userService = userService;
        _tempEstateVerificationService = tempEstateVerificationService;
        _emailService = emailService;
    }
    public async Task<ApiResponse<CorporateRegisterCommandResponse>> Handle(CorporateRegisterCommandRequest request, CancellationToken cancellationToken)
    {
        var response = await _userService.CorporateRegister(request, cancellationToken);

        if (response.IsSuccessfull)
        {
            var verificationCode = await _tempEstateVerificationService.CreateTempEstateVerification(new TempEstateVerification.Commands.CreateTempEstateVerification.CreateTempEstateVerificationCommandRequest
            {
                Email = request.Email

            }, cancellationToken);

            if (verificationCode.IsSuccessfull)
            {
                string confirmurl = $"{GeneralConstants.BaseUrl}/ConfirmEstate?email={request.Email}&code={verificationCode.Data.VerificationCode}";

                string rejecturl = $"{GeneralConstants.BaseUrl}/RejectEstate?email={request.Email}&code={verificationCode.Data.VerificationCode}";

                var emailResult = await _emailService.SendEstateConfirmationEmailAsync(new SendEstateConfirmationDto
                {
                    Address = request.Address,
                    City = request.City,
                    CompanyName = request.CompanyName,
                    District = request.District,
                    ConfirmUrl = confirmurl,
                    RejectUrl = rejecturl,
                    Email = request.Email,
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Level5CertificatePath = request.Level5CertificatePath,
                    Password = request.Password,
                    PhoneNumber = request.PhoneNumber,
                    ProfilePicturePath = request.ProfilePicturePath,
                    Reference = request.Reference,
                    TaxPlatePath = request.TaxPlatePath,
                });

                var res = await _emailService.SendAwaitingApprovalMailToEstate(request.Email);

                if (response.IsSuccessfull)
                {
                    response.Data = new CorporateRegisterCommandResponse()
                    {
                        IsSuccessfull = res
                    };

                }

            }

        }


        return response;
    }
}
