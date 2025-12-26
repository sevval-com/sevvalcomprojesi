using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;

namespace sevvalemlak.Pages.Account
{
    public class RegisterModel : PageModel
    {
        [BindProperty]
        public RegisterViewModel Input { get; set; } = new RegisterViewModel();

        public class RegisterViewModel
        {
            public string FirstName { get; set; }
            public string LastName { get; set; }

            [Required(ErrorMessage = "Email adresi gereklidir.")]
            [EmailAddress(ErrorMessage = "Geçersiz email adresi.")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Şifre gereklidir.")]
            public string Password { get; set; }

            [Required(ErrorMessage = "Şifreyi tekrar girin.")]
            [Compare("Password", ErrorMessage = "Şifreler eşleşmiyor.")]
            public string ConfirmPassword { get; set; }
        }

        public void OnGet()
        {
            ViewData["Title"] = "Kayıt Ol - Şevval Emlak";
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Kullanıcı kayıt işlemleri burada yapılacak.

            return RedirectToPage("/Account/Login");
        }
    }
}
