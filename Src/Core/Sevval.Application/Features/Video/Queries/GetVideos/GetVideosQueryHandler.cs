using MediatR;
using Sevval.Application.Interfaces.Services;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.Video.Queries.GetVideos
{
    public class GetVideosQueryHandler : IRequestHandler<GetVideosQueryRequest, ApiResponse<List<GetVideosQueryResponse>>>
    {
        private readonly IVideoService _videoService;

        public GetVideosQueryHandler(IVideoService videoService)
        {
            _videoService = videoService;
        }

        public async Task<ApiResponse<List<GetVideosQueryResponse>>> Handle(GetVideosQueryRequest request, CancellationToken cancellationToken)
        {
            return await _videoService.GetVideosAsync(request, cancellationToken);
        }
    }
}
