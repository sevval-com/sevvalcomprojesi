using System.ComponentModel.DataAnnotations;

public class KurumsalRegister
{
    [Key] // Anahtar özelliği olarak tanımlanır
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string FirstName { get; set; }

    [Required]
    [StringLength(50)]
    public string LastName { get; set; }

    [Required]
    [EmailAddress]
    public string Email { get; set; }

    [Phone]
    [StringLength(13)]
    public string Phone { get; set; }

    [Required]
    [StringLength(100)]
    public string Password { get; set; }

    [Required]
    [StringLength(100)]
    [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
    public string ConfirmPassword { get; set; }

    [StringLength(100)]
    public string CompanyName { get; set; }

    [StringLength(100)]
    public string City { get; set; }

    [StringLength(100)]
    public string District { get; set; }

    [StringLength(200)]
    public string CompanyAddress { get; set; }

    public string Reference { get; set; }

    public string Document1Path { get; set; }

    public string Document2Path { get; set; }
}
