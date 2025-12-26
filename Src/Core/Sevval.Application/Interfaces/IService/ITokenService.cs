using Sevval.Application.Dtos.Front.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sevval.Application.Features.Auth.Queries.Auth;
using Sevval.Application.Features.Common;
using Sevval.Domain.Entities;

namespace Sevval.Application.Interfaces.IService;

public interface ITokenService
{
   public Task<ApiResponse<AuthQueryResponse>> CreateToken(ApplicationUser user);
}
