using MediatR;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;

namespace Sevval.Application.Features.About.Queries.GetAboutContent;

public class GetAboutContentQueryHandler : IRequestHandler<GetAboutContentQueryRequest, ApiResponse<GetAboutContentQueryResponse>>
{
    private readonly IAboutService _aboutService;

    public GetAboutContentQueryHandler(IAboutService aboutService)
    {
        _aboutService = aboutService;
    }

    public async Task<ApiResponse<GetAboutContentQueryResponse>> Handle(GetAboutContentQueryRequest request, CancellationToken cancellationToken)
    {

        var response = await _aboutService.GetAboutContentAsync(cancellationToken);
        return response;

    }
}
