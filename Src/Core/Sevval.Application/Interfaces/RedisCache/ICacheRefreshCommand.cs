using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Interfaces.RedisCache
{
    public interface ICacheRefreshCommand
    {
        string CacheKey { get; }

    }

}
