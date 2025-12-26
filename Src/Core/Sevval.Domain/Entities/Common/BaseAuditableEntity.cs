using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Domain.Entities.Common
{
    public abstract class BaseAuditableEntity : BaseAuditableEntity<long>
    {
    }

    public abstract class BaseAuditableEntity<TId> : BaseEntity<TId>, IAuditableEntity<TId>
        where TId : IEquatable<TId>, IComparable, IComparable<TId>
    {
        protected BaseAuditableEntity(TId id) : base(id) { }

        protected BaseAuditableEntity() { }

        public DateTime? CreatedDate { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public string? LastModifiedBy { get; set; }
        public bool IsDeleted { get; set; }
    }

}
