using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sevval.Application.Dtos.Email
{
    public class ConsultantInvitationEmailDto
    {
        [Required]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string SetPasswordUrl { get; set; } = string.Empty;

        [Required]
        public string CompanyName { get; set; } = string.Empty;
    }
}
