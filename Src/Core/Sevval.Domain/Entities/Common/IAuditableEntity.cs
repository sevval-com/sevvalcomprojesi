using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Domain.Entities.Common
{
    public interface IAuditableEntity<TId> : IBaseEntity<TId>, IBaseAuditableEntity
     where TId : IEquatable<TId>, IComparable, IComparable<TId>
    { }

    public interface IAuditableEntity : IBaseEntity, IBaseAuditableEntity
    { }

}
