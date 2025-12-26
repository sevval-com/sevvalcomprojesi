using MediatR;
using Sevval.Application.Features.Common;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sevval.Application.Features.User.Queries.GetUserById
{
    public class GetUserByIdQueryRequest : IRequest<ApiResponse<GetUserByIdQueryResponse>>
    {
        public const string Route = "/api/v1/users";

        [NotMapped]
        [SwaggerIgnore]

        public string Id { get; set; }

    }
}
