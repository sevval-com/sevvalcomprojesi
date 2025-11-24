using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.SocialMedia.Queries.GetSocialMedia;

public class GetSocialMediaQueryRequest : IRequest<ApiResponse<GetSocialMediaQueryResponse>>
{
    public const string Route = "/api/v1/social-media";

}
