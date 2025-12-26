using GridBox.Solar.Domain.IRepositories;
using GridBox.Solar.Domain.IUnitOfWork;
using Microsoft.AspNetCore.Identity;
using Sevval.Application.Features.Auth.Queries.Auth;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;
using Sevval.Domain.Entities;

namespace Sevval.Infrastructure.Services
{
    public class AuthService : IAuthService
    {
        private readonly ITokenService _tokenService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IWriteRepository<UserRefreshToken> _writeRepository;
        private readonly IReadRepository<UserRefreshToken> _readRepository;
        private readonly IReadRepository<ApplicationUser> _readUserRepository;
        public AuthService(ITokenService tokenService, UserManager<ApplicationUser> userManager, IUnitOfWork unitOfWork,
            IWriteRepository<UserRefreshToken> writeRepository, IReadRepository<UserRefreshToken> readRepository, IReadRepository<ApplicationUser> readUserRepository)
        {

            _tokenService = tokenService;
            _unitOfWork = unitOfWork;
            _userManager = userManager;
            _writeRepository = writeRepository;
            _readRepository = readRepository;
            _readUserRepository = readUserRepository;
        }


        public async Task<ApiResponse<AuthQueryResponse>> CreateTokenAsync(AuthQueryRequest request, CancellationToken cancellationToken)
        {

            var user = await _readUserRepository.GetAsync(a => a.Email == request.Email);

            if (user == null)
                return new ApiResponse<AuthQueryResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Kullanıcı bulunamadı"
                };

            if (!user.LockoutEnabled || user.IsActive == "passive")
                return new ApiResponse<AuthQueryResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Kullanıcı onaylanmamış daha sonra tekrar deneyiniz."
                };

            if (!user.EmailConfirmed)
                return new ApiResponse<AuthQueryResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Lütfen e-posta adresinizi onaylayınız."
                };

            if (!await _userManager.CheckPasswordAsync(user, request.Password))
            {

                return new ApiResponse<AuthQueryResponse>
                {
                    Code = 400,
                    Data = null,
                    IsSuccessfull = false,
                    Message = "Kullanıcı bilgileri hatalı"
                };

            }

            var tokenResponse = await _tokenService.CreateToken(user);

            var userRefreshToken = await _readRepository.GetAsync(a => a.UserId == user.Id);

            if (userRefreshToken is null)
            {
                await _writeRepository.AddAsync(new UserRefreshToken
                {
                    UserId = user.Id,
                    Code = tokenResponse.Data.RefreshToken,
                    Expiration = tokenResponse.Data.RefreshTokenExpiration,

                });
            }
            else
            {
                userRefreshToken.Code = tokenResponse.Data.RefreshToken;
                userRefreshToken.Expiration = tokenResponse.Data.RefreshTokenExpiration;
                await _writeRepository.UpdateAsync(userRefreshToken);
            }

            if (await _unitOfWork.CommitAsync(cancellationToken) > 0)
            {
                return new ApiResponse<AuthQueryResponse>
                {
                    Data = tokenResponse.Data,
                    Code = 200,
                    IsSuccessfull = true,
                    Message = "Oturum Başarılı"
                };
            }

            return new ApiResponse<AuthQueryResponse>
            {
                Data = null,
                Code = 401,
                IsSuccessfull = false,
                Message = "Oturum Başarısız"
            };
        }




    }

}
