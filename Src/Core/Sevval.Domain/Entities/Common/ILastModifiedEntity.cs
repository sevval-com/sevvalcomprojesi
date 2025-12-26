using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Domain.Entities.Common
{
    public interface ILastModifiedEntity
    {
        DateTime? LastModifiedDate { get; set; }
        string? LastModifiedBy { get; set; }
    }

}
