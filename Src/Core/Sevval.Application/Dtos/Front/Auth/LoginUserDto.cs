using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Dtos.Front.Auth
{
    public class LoginUserDto
    {
        public string UserId { get; set; }
        public string Mail { get; set; }
        public string Token { get; set; }
        public string Username { get; set; }
        public string ImagePath { get; set; }
        public short? TypeId { get; set; }
        public DateTime SystemAuthTime { get; set; }
    }
}
