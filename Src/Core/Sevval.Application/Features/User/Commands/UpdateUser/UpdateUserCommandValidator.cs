using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sevval.Application.Features.Auth.Queries.Auth;

namespace Sevval.Application.Features.User.Commands.UpdateUser
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommandRequest>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta adresi zorunludur!");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre zorunludur!");
            
        }
    }
}
