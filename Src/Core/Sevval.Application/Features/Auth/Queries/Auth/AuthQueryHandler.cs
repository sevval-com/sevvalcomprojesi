using MediatR;
using Microsoft.AspNetCore.Http;
using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.Auth.Queries.Auth
{
    public class AuthQueryHandler : BaseHandler, IRequestHandler<AuthQueryRequest, ApiResponse<AuthQueryResponse>>
    {
        private readonly IAuthService _authService;

        public AuthQueryHandler(IHttpContextAccessor httpContextAccessor, IAuthService authService) : base(httpContextAccessor)
        {
            _authService = authService;
        }

        public async Task<ApiResponse<AuthQueryResponse>> Handle(AuthQueryRequest request, CancellationToken cancellationToken)
        {
            var response = await _authService.CreateTokenAsync(request, cancellationToken);
            return response;
        }
    }
}
