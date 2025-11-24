using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.Services;

namespace Sevval.Application.Features.AboutUs.Queries.GetAboutUs
{
    public class GetAboutUsQueryHandler : IRequestHandler<GetAboutUsQueryRequest, ApiResponse<GetAboutUsQueryResponse>>
    {
        private readonly IAboutUsService _aboutUsService;

        public GetAboutUsQueryHandler(IAboutUsService aboutUsService)
        {
            _aboutUsService = aboutUsService;
        }

        public async Task<ApiResponse<GetAboutUsQueryResponse>> Handle(GetAboutUsQueryRequest request, CancellationToken cancellationToken)
        {
            return await _aboutUsService.GetAboutUsAsync(request, cancellationToken);
        }
    }
}
