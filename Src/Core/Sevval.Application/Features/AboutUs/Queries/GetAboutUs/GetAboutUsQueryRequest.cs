using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.AboutUs.Queries.GetAboutUs
{
    public class GetAboutUsQueryRequest : IRequest<ApiResponse<GetAboutUsQueryResponse>>
    {
        public const string Route = "/api/v1/about-us";

        public string UserId { get; set; } 
      
    }
}
