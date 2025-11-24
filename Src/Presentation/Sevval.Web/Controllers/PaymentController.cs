using Microsoft.AspNetCore.Mvc;
using System;

namespace SevvalEmlak.Controllers
{
    public class PaymentController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Pay()
        {
            // Ödeme işlemi için gerekli parametreler burada tanımlanır.
            string merchantId = "534348";
            string merchantKey = "tXxn5WBHB9X3Funt";
            string merchantSalt = "wcQ5qi1XayCdK1N4";
            string url = "https://www.paytr.com/odeme"; // Paytr ödeme URL'si

            // Bu URL'ye form verileri gönderilecek
            return View();
        }
    }
}
