using Sevval.Application.Features.Common;
using Sevval.Application.Features.ContactInfo.Queries.GetContactInfo;

namespace Sevval.Application.Interfaces.Services
{
    public interface IContactInfoService
    {
      public  Task<ApiResponse<GetContactInfoQueryResponse>> GetContactInfoAsync(CancellationToken cancellationToken);
    }
}
