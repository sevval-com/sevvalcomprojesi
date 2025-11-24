using Microsoft.AspNetCore.Http;
using Sevval.Domain.Entities;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace sevvalemlak.Models
{
    public class SettingsViewModel
    {
        public string Id { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string CompanyName { get; set; }
        public string UserTypes { get; set; }
        public bool IsConsultant { get; set; }
        public string ProfilePicturePath { get; set; }
        public IFormFile ProfilePicture { get; set; }
        public string BannerPicturePath { get; set; }
        public IFormFile BannerPicture { get; set; }
        public List<ApplicationUser> Consultants { get; set; } = new List<ApplicationUser>();

        // Yeni eklenen �zellikler
        public string City { get; set; }
        public string District { get; set; }
        public string AcikAdres { get; set; }
    }
}
