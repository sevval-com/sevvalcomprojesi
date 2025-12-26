using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Base
{
    public class BaseHandler
    {
        public IHttpContextAccessor HttpContextAccessor { get; }

        public BaseHandler(IHttpContextAccessor httpContextAccessor)
        {
            HttpContextAccessor = httpContextAccessor;
        }
    }

}
