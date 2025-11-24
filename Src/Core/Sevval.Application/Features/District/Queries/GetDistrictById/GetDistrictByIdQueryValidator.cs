using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Features.District.Queries.GetDistrictById
{
    public class GetDistrictByIdQueryValidator : AbstractValidator<GetDistrictByIdQueryRequest>
    {
        public GetDistrictByIdQueryValidator()
        {
            RuleFor(x => x.Id).GreaterThan(0).WithMessage("Id alanı zorunludur!");
        }
    }
}
