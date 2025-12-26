using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sevval.Application.Features.Auth.Queries.Auth;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.IService
{
    public interface IAuthService
    {
        Task<ApiResponse<AuthQueryResponse>> CreateTokenAsync(AuthQueryRequest request, CancellationToken cancellationToken);
    }
}
