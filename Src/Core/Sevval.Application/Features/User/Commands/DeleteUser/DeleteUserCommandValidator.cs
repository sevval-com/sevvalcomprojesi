using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sevval.Application.Features.Auth.Queries.Auth;

namespace Sevval.Application.Features.User.Commands.DeleteUser
{
    public class DeleteUserCommandValidator : AbstractValidator<DeleteUserCommandRequest>
    {
        public DeleteUserCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty().WithMessage("Id zorunludur!");
            
        }
    }
}
