using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Features.District.Queries.GetAllDistricts
{
    public class GetAllDistrictsQueryValidator : AbstractValidator<GetAllDistrictsQueryRequest>
    {
        public GetAllDistrictsQueryValidator()
        {
            RuleFor(x => x.ProvinceName).NotEmpty().WithMessage("İl alanı zorunludur!");
        }
    }
}
