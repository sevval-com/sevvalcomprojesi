using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Features.District.Commands.CreateDistrictCommands
{
    public class CreateDistrictCommandValidator : AbstractValidator<CreateDistrictCommandRequest>
    {
        public CreateDistrictCommandValidator()
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage("İlçe adı zorunludur!");
            RuleFor(x => x.ProvinceId).GreaterThan(0).WithMessage("Şehir alanı zorunludur!");
        }
    }
}
