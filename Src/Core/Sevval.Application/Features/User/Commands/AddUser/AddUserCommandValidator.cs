using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sevval.Application.Features.Auth.Queries.Auth;

namespace Sevval.Application.Features.User.Commands.AddUser
{
    public class AddUserCommandValidator : AbstractValidator<AddUserCommandRequest>
    {
        public AddUserCommandValidator()
        {
            RuleFor(x => x.Email).NotEmpty().WithMessage("E-posta adresi zorunludur!");
            RuleFor(x => x.Password).NotEmpty().WithMessage("Şifre zorunludur!");
            RuleFor(x => x.UserType).Must(UserTypeControlForUsers).WithMessage("Geçersiz kullanıcı tipi seçtiniz.");

        }
        private static bool UserTypeControlForUsers(string arg)
        {
            return arg.Equals("carrier") || arg.Equals("broker") || arg.Equals("admin");
        }
    }
}
