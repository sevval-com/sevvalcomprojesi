using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Interfaces.IService.Common
{
    public interface IDateTimeService
    {
        DateTime Now { get; }
        DateTime UtcNow { get; }
    }

}
