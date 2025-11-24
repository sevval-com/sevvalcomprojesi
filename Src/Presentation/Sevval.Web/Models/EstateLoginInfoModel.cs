namespace sevvalemlak.Models
{
    public class EstateLoginInfoModel
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string CompanyName { get; set; }
        public string City { get; set; }
        public string District { get; set; }
        public string Address { get; set; }
        public IFormFile Level5Certificate { get; set; }
        public IFormFile TaxPlate { get; set; }
        public string Level5CertificatePath { get; set; }
        public string TaxPlatePath { get; set; }
        public string Reference { get; set; }
        public IFormFile ProfilePicture { get; set; }
        public string ProfilePicturePath { get; set; }

    }
}
