using Microsoft.AspNetCore.Mvc;

namespace sevvalemlak.Controllers
{
    public class TestHamzaController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Yukle(IFormFile[] fromFiles)
        {
            if (fromFiles.Length <= 0)
            {
                return BadRequest();
            }
            List<string> strings = new List<string>();

            foreach (var item in fromFiles)
            {
                var uniqName = Guid.NewGuid();

                var savePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images/uploads");

                var uniqFileName = Path.Combine(savePath, $"{uniqName}_{item.FileName}");


                using (var stream = new FileStream(uniqFileName, FileMode.Create))
                {
                    await item.CopyToAsync(stream);
                }

                string photoUrl = $"/images/uploads/{uniqName}_{uniqFileName}";


                strings.Add(photoUrl);

            }

            return Ok();
        }
    }
}
