using Sevval.Application.Dtos.Front.Auth;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.User.Queries.GetAllUsers;
using Sevval.Application.Features.User.Commands.AddUser;
using Sevval.Application.Features.User.Commands.DeleteUser;
using Sevval.Application.Features.User.Queries.GetUserById;
using Sevval.Application.Features.User.Commands.UpdateUser;
using Sevval.Application.Features.User.Commands.UpdateUserProfile;
using Sevval.Application.Features.User.Queries.CheckUserExists;
using Sevval.Application.Features.User.Commands.CorporateRegister;
using Sevval.Application.Features.User.Commands.IndividualRegister;
using Sevval.Application.Features.User.Commands.ForgottenPassword;
using Sevval.Application.Features.User.Commands.SendNewCode;
using Sevval.Application.Features.User.Commands.ConfirmEstate;
using Sevval.Application.Features.User.Commands.RejectEstate;
using Sevval.Application.Features.User.Commands.LoginWithSocialMedia;
using Sevval.Application.Features.User.Commands.RefreshToken;
using Sevval.Application.Features.User.Commands.IndividualUpdate;
using Sevval.Application.Features.User.Commands.CorporateUpdate;
using Sevval.Application.Features.User.Commands.UpdatePassword;


namespace Sevval.Application.Interfaces.IService
{
    public interface IUserService
    {
        Task<ApiResponse<AddUserCommandResponse>> AddUser(AddUserCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<CheckUserExistsQueryResponse>> CheckUserExists(CheckUserExistsQueryRequest request);
        Task<ApiResponse<DeleteUserCommandResponse>> DeleteUser(DeleteUserCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<IList<GetAllUsersQueryResponse>>> GetAllUsers(GetAllUsersQueryRequest request);
        Task<ApiResponse<GetUserByIdQueryResponse>> GetUser(GetUserByIdQueryRequest request);
        Task<ApiResponse<LoginUserDto>> LoginAsync(LoginDto LoginDto);
        Task<ApiResponse<CorporateRegisterCommandResponse>> CorporateRegister(CorporateRegisterCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<UpdateUserCommandResponse>> UpdateUser(UpdateUserCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<UpdateUserProfileCommandResponse>> UpdateUserProfile(UpdateUserProfileCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<IndividualRegisterCommandResponse>> IndividualRegister(IndividualRegisterCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<bool>> Logout();
        Task<ApiResponse<ForgottenPasswordCommandResponse>> ForgotPassword(ForgottenPasswordCommandRequest request);
        Task<ApiResponse<SendNewCodeCommandResponse>> SendNewCode(SendNewCodeCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<ConfirmEstateCommandResponse>> ConfirmEstate(ConfirmEstateCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<RejectEstateCommandResponse>> RejectEstate(RejectEstateCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<LoginWithSocialMediaCommandResponse>> LoginWithSocialMedia(LoginWithSocialMediaCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<RefreshTokenCommandResponse>> RefreshToken(RefreshTokenCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<IndividualUpdateCommandResponse>> IndividualUpdate(IndividualUpdateCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<CorporateUpdateCommandResponse>> CorporateUpdate(CorporateUpdateCommandRequest request, CancellationToken cancellationToken);
        Task<ApiResponse<UpdatePasswordCommandResponse>> UpdatePassword(UpdatePasswordCommandRequest request, CancellationToken cancellationToken);
    }
}
