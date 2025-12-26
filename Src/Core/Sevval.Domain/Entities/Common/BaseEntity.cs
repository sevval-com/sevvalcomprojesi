using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Domain.Entities.Common
{
    public abstract class BaseEntity : BaseEntity<long> { }

    public abstract class BaseEntity<TId> : IEquatable<BaseEntity<TId>>, IBaseEntity<TId>
        where TId :
        IEquatable<TId>,
        IComparable,
        IComparable<TId>
    {
        protected BaseEntity(TId id) => Id = id;

        protected BaseEntity() { }

        public TId Id { get; set; } = default!;

        public static bool operator ==(BaseEntity<TId>? first, BaseEntity<TId>? second) =>
               first is not null && second is not null && first.Equals(second);

        public static bool operator !=(BaseEntity<TId>? first, BaseEntity<TId>? second) =>
            !(first == second);

        public bool Equals(BaseEntity<TId>? other)
        {
            if (other is null)
            {
                return false;
            }

            if (other.GetType() != GetType())
            {
                return false;
            }

            return other.Id.Equals(Id);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            if (obj is not BaseEntity<TId> entity)
            {
                return false;
            }

            return entity.Id.Equals(Id);
        }

        public override int GetHashCode() => Id.GetHashCode() * 28;
    }
}
