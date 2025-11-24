using Sevval.Application.Base;
using Sevval.Application.Features.Common;
using Sevval.Application.Interfaces.IService;
using MediatR;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Features.District.Commands.CreateDistrictCommands
{
    public class CreateDistrictCommandHandler : BaseHandler, IRequestHandler<CreateDistrictCommandRequest, ApiResponse<CreateDistrictCommandResponse>>
    {
        private readonly IDistrictService _districtService;

        public CreateDistrictCommandHandler(IHttpContextAccessor httpContextAccessor, IDistrictService districtService) : base(httpContextAccessor)
        {
            _districtService = districtService;
        }

        public async Task<ApiResponse<CreateDistrictCommandResponse>> Handle(CreateDistrictCommandRequest request, CancellationToken cancellationToken)
        {
            var response = await _districtService.CreateDistrict(request, cancellationToken);

            return response;
        }
    }
}
