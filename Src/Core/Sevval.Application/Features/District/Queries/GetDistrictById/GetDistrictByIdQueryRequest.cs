using Sevval.Application.Features.Common;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Features.District.Queries.GetDistrictById
{
    public class GetDistrictByIdQueryRequest : IRequest<ApiResponse<GetDistrictByIdQueryResponse>>
    {
        public const string Route = "/api/v1/districts/{id}";

        public int Id { get; set; }
    }
}
