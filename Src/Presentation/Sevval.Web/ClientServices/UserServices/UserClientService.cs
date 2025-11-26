using Sevval.Application.Constants;
using Sevval.Application.Features.Common;
using Sevval.Application.Features.User.Commands.ConfirmEstate;
using Sevval.Application.Features.User.Commands.CorporateRegister;
using Sevval.Application.Features.User.Commands.DeleteUser;
using Sevval.Application.Features.User.Commands.ForgottenPassword;
using Sevval.Application.Features.User.Commands.IndividualRegister;
using Sevval.Application.Features.User.Commands.LoginWithSocialMedia;
using Sevval.Application.Features.User.Commands.RejectEstate;
using Sevval.Application.Features.User.Commands.SendNewCode;
using System.Net.Http.Headers;

namespace sevvalemlak.csproj.ClientServices.UserServices
{
    public class UserClientService : IUserClientService
    {
        private readonly HttpClient _httpClient;
        public UserClientService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }


        public async Task<ApiResponse<CorporateRegisterCommandResponse>> CorporateRegister(CorporateRegisterCommandRequest request, CancellationToken cancellationToken)
        {
            var content = new MultipartFormDataContent();
            content.Add(new StringContent(request.FirstName), "FirstName");
            content.Add(new StringContent(request.LastName), "LastName");
            content.Add(new StringContent(request.Email), "Email");
            content.Add(new StringContent(request.PhoneNumber), "PhoneNumber");
            content.Add(new StringContent(request.Password), "Password");
            content.Add(new StringContent(request.ConfirmPassword), "ConfirmPassword");

            if (request.Level5Certificate is not null)
            {
                var streamContent = new StreamContent(request.Level5Certificate.OpenReadStream());
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "Level5Certificate",
                    FileName = request.Level5Certificate.FileName
                };
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(request.Level5Certificate.ContentType);
                content.Add(streamContent);
            }

            if (request.TaxPlate is not null)
            {
                var streamContent = new StreamContent(request.TaxPlate.OpenReadStream());
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "TaxPlate",
                    FileName = request.TaxPlate.FileName
                };
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(request.TaxPlate.ContentType);
                content.Add(streamContent);
            }

            if (request.ProfilePicture is not null)
            {
                var streamContent = new StreamContent(request.ProfilePicture.OpenReadStream());
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "ProfilePicture",
                    FileName = request.ProfilePicture.FileName
                };
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(request.ProfilePicture.ContentType);
                content.Add(streamContent);
            }

            content.Add(new StringContent(request.CompanyName), "CompanyName");
            content.Add(new StringContent(request.City), "City");
            content.Add(new StringContent(request.District), "District");
            content.Add(new StringContent(request.UserTypes), "UserTypes");
            content.Add(new StringContent(request.Address), "Address");


            var response = await _httpClient.PostAsync(GeneralConstants.BaseClientUrl + CorporateRegisterCommandRequest.Route, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ApiResponse<CorporateRegisterCommandResponse>>(cancellationToken: cancellationToken);

            }

            return new ApiResponse<CorporateRegisterCommandResponse>
            {
                IsSuccessfull = false,
                Message = "Kayıt işlemi başarısız oldu.",
                Data = null
            };
        }
        public async Task<ApiResponse<IndividualRegisterCommandResponse>> IndividualRegister(IndividualRegisterCommandRequest request, CancellationToken cancellationToken)
        {
            var content = new MultipartFormDataContent();

            content.Add(new StringContent(request.FirstName), "FirstName");
            content.Add(new StringContent(request.LastName), "LastName");
            content.Add(new StringContent(request.Email), "Email");
            content.Add(new StringContent(request.PhoneNumber), "PhoneNumber");
            content.Add(new StringContent(request.Password), "Password");

            content.Add(new StringContent(request.ConfirmPassword), "ConfirmPassword");

            if (request.ProfilePicture is not null)
            {
                var streamContent = new StreamContent(request.ProfilePicture.OpenReadStream());
                streamContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data")
                {
                    Name = "ProfilePicture",
                    FileName = request.ProfilePicture.FileName
                };
                streamContent.Headers.ContentType = new MediaTypeHeaderValue(request.ProfilePicture.ContentType);
                content.Add(streamContent);
            }

            var response = await _httpClient.PostAsync(GeneralConstants.BaseClientUrl + IndividualRegisterCommandRequest.Route, content, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ApiResponse<IndividualRegisterCommandResponse>>(cancellationToken: cancellationToken);

            }

            return new ApiResponse<IndividualRegisterCommandResponse>
            {
                IsSuccessfull = false,
                Message = "Kayıt işlemi başarısız oldu.",
                Data = null
            };
        }


        public async Task<ApiResponse<ConfirmEstateCommandResponse>> ConfirmEstate(ConfirmEstateCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(GeneralConstants.BaseClientUrl + ConfirmEstateCommandRequest.Route, request, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponse<ConfirmEstateCommandResponse>>(cancellationToken: cancellationToken);
        }

        public async Task<ApiResponse<RejectEstateCommandResponse>> RejectEstate(RejectEstateCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(GeneralConstants.BaseClientUrl + RejectEstateCommandRequest.Route, request, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponse<RejectEstateCommandResponse>>(cancellationToken: cancellationToken);
        }

        public async Task<ApiResponse<SendNewCodeCommandResponse>> SendNewCode(SendNewCodeCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(GeneralConstants.BaseClientUrl + SendNewCodeCommandRequest.Route, request, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponse<SendNewCodeCommandResponse>>(cancellationToken: cancellationToken);
        }

        public async Task<ApiResponse<ForgottenPasswordCommandResponse>> ForgottenPassword(ForgottenPasswordCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(GeneralConstants.BaseClientUrl + ForgottenPasswordCommandRequest.Route, request, cancellationToken);
            return await response.Content.ReadFromJsonAsync<ApiResponse<ForgottenPasswordCommandResponse>>(cancellationToken: cancellationToken);
        }

        public async Task<ApiResponse<LoginWithSocialMediaCommandResponse>> SocialRegister(LoginWithSocialMediaCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _httpClient.PostAsJsonAsync(GeneralConstants.BaseClientUrl + LoginWithSocialMediaCommandRequest.Route, request, cancellationToken);

            if (response.IsSuccessStatusCode)
            {
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadFromJsonAsync<ApiResponse<LoginWithSocialMediaCommandResponse>>(cancellationToken: cancellationToken);

            }

            return new ApiResponse<LoginWithSocialMediaCommandResponse>
            {
                IsSuccessfull = false,
                Message = "Kayıt işlemi başarısız oldu.",
                Data = null
            };
        }

        public async Task<ApiResponse<DeleteUserCommandResponse>> DeleteUser(string userId)
        {
            try
            {
                var response = await _httpClient.DeleteAsync(
                    GeneralConstants.BaseClientUrl + DeleteUserCommandRequest.Route + "/" + userId,
                    CancellationToken.None
                );

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ApiResponse<DeleteUserCommandResponse>>();
                }

                return new ApiResponse<DeleteUserCommandResponse>
                {
                    IsSuccessfull = false,
                    Message = "Hesap silme işlemi başarısız oldu.",
                    Data = null,
                    Code = (int)response.StatusCode
                };
            }
            catch (Exception ex)
            {
                return new ApiResponse<DeleteUserCommandResponse>
                {
                    IsSuccessfull = false,
                    Message = $"API çağrısı sırasında hata: {ex.Message}",
                    Data = null,
                    Code = 500
                };
            }
        }


    }
}
