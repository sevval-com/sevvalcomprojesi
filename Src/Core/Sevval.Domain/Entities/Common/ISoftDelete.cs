using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Domain.Entities.Common
{
    public interface ISoftDelete
    {
        bool IsDeleted { get; set; }
    }

}
