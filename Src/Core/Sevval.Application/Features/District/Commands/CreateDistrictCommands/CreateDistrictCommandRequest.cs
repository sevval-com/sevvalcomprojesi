using Sevval.Application.Features.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Features.District.Commands.CreateDistrictCommands
{
    public class CreateDistrictCommandRequest : IRequest<ApiResponse<CreateDistrictCommandResponse>>
    {
        public const string Route = "/api/v1/districts";
        //public string CacheKey => "Districts";

        public string Name { get; set; }
        public string Code { get; set; }
        public int ProvinceId { get; set; }
    }
}
