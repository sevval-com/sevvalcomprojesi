using Sevval.Domain.Entities.Common;

namespace Sevval.Domain.Entities;

public class TempEstateVerification : BaseAuditableEntity<int>
{
    public string Email { get; set; }
    public string Code { get; set; }
    public DateTime Expiration { get; set; }
}
