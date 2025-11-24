using MediatR;
using Sevval.Application.Features.Common;

namespace Sevval.Application.Features.ContactInfo.Queries.GetContactInfo;

public class GetContactInfoQueryRequest : IRequest<ApiResponse<GetContactInfoQueryResponse>>
{
    public const string Route = "/api/v1/contact-info";

}
