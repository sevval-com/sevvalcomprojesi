using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Video.Queries.GetVideos;

public class GetVideosQueryRequest : IRequest<ApiResponse<List<GetVideosQueryResponse>>>
{
    public const string Route = "/api/v1/videos";

}
