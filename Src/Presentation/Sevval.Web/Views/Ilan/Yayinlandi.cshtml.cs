using Microsoft.AspNetCore.Mvc.RazorPages;

namespace YourProjectNamespace.Views.Ilan // Namespace'i kendi projenize göre ayarlayın
{
    public class YayinlandiModel : PageModel
    {
        public int Id { get; set; } // İlanın ID'sini tutmak için

        public void OnGet(int id) // İlanın detaylarını almak için
        {
            Id = id;
            // Diğer gerekli işlemleri yapabilirsiniz.
        }
    }
}
