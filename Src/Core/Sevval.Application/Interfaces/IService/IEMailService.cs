using Sevval.Application.Dtos.Email;

namespace Sevval.Application.Interfaces.IService
{
    public interface IEMailService
    {
        Task<bool> SendAwaitingApprovalMailToEstate(string email);
        Task<bool> SendConfirmeInformationMailToEstate(string email);
        Task<bool> SendConsultantInvitationEmailAsync(ConsultantInvitationEmailDto model);
        Task<bool> SendEmailAsync(string to, string subject, string body);
        Task<bool> SendEstateConfirmationEmailAsync(SendEstateConfirmationDto model);
        Task<bool> SendPasswordResetEmailAsync(ForgotPasswordViewDto model, string code);
        Task<bool> SendRejectedMailToEstate(string email);
        Task<bool> SendVerificationEmailAsync(SendVerifyEmailDto model);
    }
}
