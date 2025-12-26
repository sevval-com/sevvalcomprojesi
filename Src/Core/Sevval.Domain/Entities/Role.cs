using Microsoft.AspNetCore.Identity;
using Sevval.Domain.Entities.Common;

namespace Sevval.Domain.Entities
{
    public partial class Role : IdentityRole//, IAuditableEntity<string>
    {
       
        //public DateTime? CreatedDate { get; set; }
        //public string CreatedBy { get; set; }
        //public DateTime? LastModifiedDate { get; set; }
        //public string LastModifiedBy { get; set; }
        //public bool IsDeleted { get; set; }
    }
}
