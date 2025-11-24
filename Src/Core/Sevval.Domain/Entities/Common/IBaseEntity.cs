using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Domain.Entities.Common
{
    public interface IBaseEntity : IBaseEntity<long> { }

    public interface IBaseEntity<TId> where TId : IEquatable<TId>, IComparable, IComparable<TId>
    {
        TId Id { get; set; }
      
    }

}
