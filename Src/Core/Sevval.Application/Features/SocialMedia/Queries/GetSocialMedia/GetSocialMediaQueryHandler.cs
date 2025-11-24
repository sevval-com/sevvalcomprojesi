using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.Services;

namespace Sevval.Application.Features.SocialMedia.Queries.GetSocialMedia
{
    public class GetSocialMediaQueryHandler : IRequestHandler<GetSocialMediaQueryRequest, ApiResponse<GetSocialMediaQueryResponse>>
    {
        private readonly ISocialMediaService _socialMediaService;

        public GetSocialMediaQueryHandler(ISocialMediaService socialMediaService)
        {
            _socialMediaService = socialMediaService;
        }

        public async Task<ApiResponse<GetSocialMediaQueryResponse>> Handle(GetSocialMediaQueryRequest request, CancellationToken cancellationToken)
        {
            return await _socialMediaService.GetSocialMediaAsync(cancellationToken);
        }
    }
}
