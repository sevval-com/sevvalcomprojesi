using Sevval.Application.Features.AboutUs.Queries.GetAboutUs;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Interfaces.Services;

public interface IAboutUsService
{
    Task<ApiResponse<GetAboutUsQueryResponse>> GetAboutUsAsync(GetAboutUsQueryRequest request, CancellationToken cancellationToken);
}
