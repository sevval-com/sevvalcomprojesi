using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Features.User.Queries.GetAllUsers
{
    public class GetAllUsersQueryResponse
    {
        public Guid Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public string Email { get; set; }
        public bool Status { get; set; }
        public bool InReferenceProgram { get; set; }

    }
}
