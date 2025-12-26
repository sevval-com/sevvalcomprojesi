using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;
namespace Sevval.Application.Utilities;

public static class SignHelper
{
    public static SecurityKey GetSymmetricSecurityKey(string securityKey)
    {
        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(securityKey));
    }
}
