using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace sevvalemlak.Views.Account
{
    public class LoginModel : PageModel
    {
        [BindProperty]
        public InputModel Input { get; set; }

        public string ReturnUrl { get; set; }

        public class InputModel
        {
            [Required]
            [EmailAddress]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            public string Password { get; set; }
        }

        public void OnGet(string? returnUrl = null)
        {
            ReturnUrl = returnUrl ?? Url.Content("~/");

            // Bu satırda ViewData kullanımı
            ViewData["Title"] = "Giriş Yap";
        }

        public IActionResult OnPost()
        {
            if (ModelState.IsValid)
            {
                // Login işlemi için örnek bir kontrol
                if (Input.Email == "test@example.com" && Input.Password == "Password123")
                {
                    return RedirectToPage("/Index");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Geçersiz giriş denemesi.");
                }
            }

            // Geçersiz model durumunda sayfa yeniden yüklenir.
            return Page();
        }
    }
}
