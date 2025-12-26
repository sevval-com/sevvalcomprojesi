using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace sevvalemlak.Models
{
    public class ChangeCredentialsViewModel
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? IPAddress { get; set; }
        public string? Password { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;
        public string? ProfilePicturePath { get; set; }
        public IFormFile? ProfilePicture { get; set; }
        public string? BannerPicturePath { get; set; }
        public IFormFile? BannerPicture { get; set; }
        public string? UserTypes { get; internal set; }
        public string? CompanyName { get; set; }
    }
}
