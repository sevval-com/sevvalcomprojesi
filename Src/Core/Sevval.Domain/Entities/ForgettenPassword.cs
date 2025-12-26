using Sevval.Domain.Entities.Common;

namespace Sevval.Domain.Entities;

public class ForgettenPassword : BaseAuditableEntity<int>
{
    public string Code { get; set; }
    public string UserId { get; set; }
    public ApplicationUser User { get; set; }
    public DateTime ExpireDate { get; set; }
}
