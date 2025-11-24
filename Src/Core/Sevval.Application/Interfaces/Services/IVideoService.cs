using Sevval.Application.Features.Video.Queries.GetVideos;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.Services
{
    public interface IVideoService
    {
        Task<ApiResponse<List<GetVideosQueryResponse>>> GetVideosAsync(GetVideosQueryRequest request, CancellationToken cancellationToken);
    }
}
