using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Sevval.Application.Features.Auth.Queries.Auth;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Utilities;
using Sevval.Domain.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Sevval.Infrastructure.Services
{
    public class TokenService : ITokenService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly CustomTokenOption _customTokenOption;
        private readonly IMapper _mapper;
        public TokenService(UserManager<ApplicationUser> userManager, IOptions<CustomTokenOption> options, IMapper mapper)
        {
            _userManager = userManager;
            _customTokenOption = options.Value;
            _mapper = mapper;
        }

        public async Task<ApiResponse<AuthQueryResponse>> CreateToken(ApplicationUser user)
        {
            var accessTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.AccessTokenExpiration);
            var refreshTokenExpiration = DateTime.Now.AddMinutes(_customTokenOption.RefreshTokenExpiration);

            var securityKey = SignHelper.GetSymmetricSecurityKey(_customTokenOption.SecurityKey);

            SigningCredentials signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);

            JwtSecurityToken jwtSecurityToken = new JwtSecurityToken(
                issuer: _customTokenOption.Issuer,
                expires: accessTokenExpiration,
                notBefore: DateTime.Now,
                claims: await GetClaims(user, _customTokenOption.Audiences),
                signingCredentials: signingCredentials
                );

            var handler = new JwtSecurityTokenHandler();
            var token = handler.WriteToken(jwtSecurityToken);

            if (string.IsNullOrEmpty(token))
            {
                return new ApiResponse<AuthQueryResponse>
                {
                    Code = 401,
                    Data = null,
                    IsSuccessfull = true,
                    Message = "Giriş Bilgileri Hatalı"
                };
            }

            var tokenDto = new AuthQueryResponse
            {
                AccessToken = token,
                AccessTokenExpiration = accessTokenExpiration,
                RefreshToken = CreateRefreshToken(),
                RefreshTokenExpiration = refreshTokenExpiration

            };

            return new ApiResponse<AuthQueryResponse>
            {
                Code = 200,
                Data = tokenDto,
                IsSuccessfull = true,
                Message = "Giriş Başarılı"
            };

        }
        private string CreateRefreshToken()
        {
            var numberByte = new Byte[32];
            using var rnd = RandomNumberGenerator.Create();
            rnd.GetBytes(numberByte);

            return Convert.ToBase64String(numberByte);
        }




        private async Task<IEnumerable<Claim>> GetClaims(ApplicationUser user, List<string> audiences)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var userList = new List<Claim>()
            {
                new Claim(ClaimTypes.NameIdentifier,user.Id),
                new Claim(JwtRegisteredClaimNames.Email,user.Email),
                new Claim(ClaimTypes.Name,user.UserName),

                new Claim(JwtRegisteredClaimNames.Jti,Guid.NewGuid().ToString()),


            };


            userList.AddRange(audiences.Select(a => new Claim(JwtRegisteredClaimNames.Aud, a)));
            userList.AddRange(userRoles.Select(a => new Claim(ClaimTypes.Role, a)));

            return userList;
        }


    }

}
