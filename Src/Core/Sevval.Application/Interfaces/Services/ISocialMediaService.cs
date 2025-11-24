using Sevval.Application.Features.Common;
using Sevval.Application.Features.SocialMedia.Queries.GetSocialMedia;

namespace Sevval.Application.Interfaces.Services;

public interface ISocialMediaService
{
    Task<ApiResponse<GetSocialMediaQueryResponse>> GetSocialMediaAsync(CancellationToken cancellationToken);
}
