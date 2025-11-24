namespace sevvalemlak.Models
{
    public class UserViewModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string? ProfilePicturePath { get; set; }  // Add this line
        public string? UserTypes { get; internal set; }

        public string? CompanyName { get; set; }
        public string? City { get; set; }        // Şehir
        public string? District { get; set; }
    }
}

