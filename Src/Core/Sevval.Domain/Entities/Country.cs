using Sevval.Domain.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace Sevval.Domain.Entities;

public class Country:BaseAuditableEntity<short>
{

    [StringLength(100)]

    public string Name { get; set; } // Ülke adı Türkiye

    [StringLength(10)]

    public string Code { get; set; } // Ülke kodu TR
}
