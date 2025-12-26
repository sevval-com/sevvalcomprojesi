using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Dtos.Front.Auth
{
    public class RefreshTokenResponseDto
    {
        public string AccessToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
    }
}
