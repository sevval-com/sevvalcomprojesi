using Sevval.Domain.Entities.Common;

namespace Sevval.Domain.Entities;

public class UserRefreshToken : BaseAuditableEntity<int>
{
    public string UserId { get; set; }
    public string Code { get; set; }
    public DateTime Expiration { get; set; }
}
