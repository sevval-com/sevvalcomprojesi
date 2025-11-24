using Sevval.Application.Features.Common;
using Sevval.Application.Features.User.Commands.ConfirmEstate;
using Sevval.Application.Features.User.Commands.CorporateRegister;
using Sevval.Application.Features.User.Commands.ForgottenPassword;
using Sevval.Application.Features.User.Commands.IndividualRegister;
using Sevval.Application.Features.User.Commands.LoginWithSocialMedia;
using Sevval.Application.Features.User.Commands.RejectEstate;
using Sevval.Application.Features.User.Commands.SendNewCode;

namespace sevvalemlak.csproj.ClientServices.UserServices
{
    public interface IUserClientService
    {
        Task<ApiResponse<ConfirmEstateCommandResponse>> ConfirmEstate(ConfirmEstateCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<CorporateRegisterCommandResponse>> CorporateRegister(CorporateRegisterCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<ForgottenPasswordCommandResponse>> ForgottenPassword(ForgottenPasswordCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<IndividualRegisterCommandResponse>> IndividualRegister(IndividualRegisterCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<RejectEstateCommandResponse>> RejectEstate(RejectEstateCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<SendNewCodeCommandResponse>> SendNewCode(SendNewCodeCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<LoginWithSocialMediaCommandResponse>> SocialRegister(LoginWithSocialMediaCommandRequest request, CancellationToken cancellationToken);
    }
}