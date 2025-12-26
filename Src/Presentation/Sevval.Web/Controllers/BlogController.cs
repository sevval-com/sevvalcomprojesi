using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sevval.Persistence.Context;
using System.Text.RegularExpressions; // Slug oluşturmak için eklendi
using YourProjectName.Models; // Blog modelinizin bulunduğu namespace

namespace YourProjectName.Controllers
{
    public class BlogController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; // Görsel yükleme için
        private readonly ILogger<BlogController> _logger; // Hata ayıklama için eklendi

        public BlogController(ApplicationDbContext context, IWebHostEnvironment hostEnvironment, ILogger<BlogController> logger)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
            _logger = logger; // Logger başlatıldı
        }

        // GET: Blog
        // Tüm blog yazılarını listeler (ana blog sayfası)
        public async Task<IActionResult> Index()
        {
            // Tüm blogları yayın tarihine göre tersten sırala
            var blogs = await _context.Blogs.OrderByDescending(b => b.PublishDate).ToListAsync();

            // Blogları kategorilere göre gruplandır
            // Her bir anahtar bir kategori adı olacak ve değeri o kategoriye ait blogların listesi olacak
            var blogsByCategory = blogs
                .GroupBy(b => b.Category)
                .ToDictionary(g => g.Key, g => g.OrderByDescending(b => b.PublishDate).ToList());

            // ViewModel oluşturarak hem en son blogları hem de kategorilere göre gruplanmış blogları gönder
            var viewModel = new BlogIndexViewModel
            {
                AllBlogs = (IEnumerable<Blog>)blogs,
                BlogsByCategory = (Dictionary<string, List<Blog>>)blogsByCategory // Buraya açık tür dönüşümü ekledik
            };

            return View(viewModel);
        }

        // GET: Blog/Details/5 (Bu action hala kullanılabilir, ancak yeni route'a yönlendireceğiz)
        // Tek bir blog yazısını detaylı gösterir
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            // SEO dostu URL'ye yönlendirme yap
            return RedirectToAction("DetailsBySlug", new { titleSlug = GenerateSlug(blog.Title) });
        }

        // GET: Blog/{titleSlug} - Yeni SEO dostu detay sayfası action'ı
        public async Task<IActionResult> DetailsBySlug(string titleSlug)
        {
            if (string.IsNullOrEmpty(titleSlug))
            {
                return NotFound();
            }

            // Slug'ı kullanarak blogu bul
            // Başlıklar tamamen benzersiz olmayabilir, bu yüzden slug'ı tam eşleşme olarak arıyoruz.
            // Daha sağlam bir çözüm için Blog modeline bir 'Slug' alanı eklemek ve onu veritabanında saklamak daha iyi olur.
            var blogs = await _context.Blogs.ToListAsync();
            var blog = blogs.FirstOrDefault(b => GenerateSlug(b.Title) == titleSlug);

            if (blog == null)
            {
                return NotFound();
            }

            return View("Details", blog); // Mevcut Details.cshtml view'ını kullan
        }

        // GET: Blog/Create
        // Yeni blog yazısı oluşturma paneli
        public IActionResult Create()
        {
            return View();
        }

        // POST: Blog/Create
        // Yeni blog yazısını kaydeder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,Content,Author,Category,ShortDescription")] Blog blog, IFormFile imageFile)
        {
            ModelState.Remove("PublishDate"); // PublishDate zorunluluğunu model state'ten çıkar

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    string fileName = Guid.NewGuid().ToString() + Path.GetExtension(imageFile.FileName);
                    string path = Path.Combine(wwwRootPath, "images", "blog", fileName);

                    using (var fileStream = new FileStream(path, FileMode.Create))
                    {
                        await imageFile.CopyToAsync(fileStream);
                    }
                    blog.ImageUrl = "/images/blog/" + fileName;
                }

                blog.PublishDate = DateTime.Now;
                _context.Add(blog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(blog);
        }

        // GET: Blog/Edit/5
        // Blog yazısı düzenleme paneli
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FindAsync(id);
            if (blog == null)
            {
                return NotFound();
            }
            return View(blog);
        }

        // POST: Blog/Edit/5
        // Blog yazısını günceller
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Content,ImageUrl,PublishDate,Author,Category,ShortDescription")] Blog blog, IFormFile newImageFile)
        {
            if (id != blog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Yeni görsel yükleme işlemi
                    if (newImageFile != null)
                    {
                        // Eski görseli sil (isteğe bağlı)
                        if (!string.IsNullOrEmpty(blog.ImageUrl))
                        {
                            string oldImagePath = Path.Combine(_hostEnvironment.WebRootPath, blog.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        string wwwRootPath = _hostEnvironment.WebRootPath;
                        string fileName = Guid.NewGuid().ToString() + Path.GetExtension(newImageFile.FileName);
                        string path = Path.Combine(wwwRootPath, "images", "blog", fileName);

                        using (var fileStream = new FileStream(path, FileMode.Create))
                        {
                            await newImageFile.CopyToAsync(fileStream);
                        }
                        blog.ImageUrl = "/images/blog/" + fileName;
                    }

                    _context.Update(blog);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Blog yazısı başarıyla güncellendi: {Title}", blog.Title);
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BlogExists(blog.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            else
            {
                // ModelState.IsValid false ise, hataları logla
                foreach (var modelStateEntry in ModelState.Values)
                {
                    foreach (var error in modelStateEntry.Errors)
                    {
                        _logger.LogError("Model hatası (Düzenleme): {ErrorMessage}", error.ErrorMessage);
                    }
                }
                _logger.LogWarning("Blog yazısı güncellenemedi: Doğrulama hataları mevcut.");
            }
            return View(blog);
        }

        // GET: Blog/Delete/5
        // Blog yazısı silme onayı
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var blog = await _context.Blogs.FirstOrDefaultAsync(m => m.Id == id);
            if (blog == null)
            {
                return NotFound();
            }

            return View(blog);
        }

        // POST: Blog/Delete/5
        // Blog yazısını siler
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var blog = await _context.Blogs.FindAsync(id);
            if (blog != null)
            {
                // Görseli sunucudan sil (isteğe bağlı)
                if (!string.IsNullOrEmpty(blog.ImageUrl))
                {
                    string imagePath = Path.Combine(_hostEnvironment.WebRootPath, blog.ImageUrl.TrimStart('/'));
                    if (System.IO.File.Exists(imagePath))
                    {
                        System.IO.File.Delete(imagePath);
                    }
                }
                _context.Blogs.Remove(blog);
                _logger.LogInformation("Blog yazısı başarıyla silindi: {Title}", blog.Title);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BlogExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }

        // Başlıktan SEO dostu bir slug oluşturan yardımcı metot
        private string GenerateSlug(string title)
        {
            string slug = title.ToLower();
            // Türkçeye özgü karakterleri İngilizce karşılıklarına çevir
            slug = slug.Replace("ç", "c")
                       .Replace("ğ", "g")
                       .Replace("ı", "i")
                       .Replace("ö", "o")
                       .Replace("ş", "s")
                       .Replace("ü", "u");

            // Geçersiz karakterleri kaldır ve boşlukları tire ile değiştir
            slug = Regex.Replace(slug, @"[^a-z0-9\s-]", ""); // Sadece harf, rakam, boşluk ve tire kalır
            slug = Regex.Replace(slug, @"\s+", "-").Trim(); // Birden fazla boşluğu tek tireye çevir
            slug = Regex.Replace(slug, @"-+", "-"); // Birden fazla tireyi tek tireye çevir
            return slug;
        }
    }

    // BlogIndexViewModel'i tanımlayın (genellikle Models klasöründe ayrı bir dosya olarak tutulur)
    public class BlogIndexViewModel
    {
        public IEnumerable<Blog> AllBlogs { get; set; }
        public Dictionary<string, List<Blog>> BlogsByCategory { get; set; }
    }
}
