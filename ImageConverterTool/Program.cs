using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats.Webp;
using SixLabors.ImageSharp.Processing;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 1)
        {
            Console.WriteLine("Kullanim: ImageConverter <wwwroot_path>");
            return;
        }

        string wwwrootPath = args[0];
        int quality = 75;
        int convertedCount = 0;
        int errorCount = 0;

        Console.WriteLine($"wwwroot yolu: {wwwrootPath}");
        Console.WriteLine($"WebP kalitesi: {quality}");
        Console.WriteLine("Donusturme basladi...\n");

        // .jpg, .jpeg, .png dosyalarını bul
        var extensions = new[] { "*.jpg", "*.jpeg", "*.png" };
        var imageFiles = new List<string>();

        foreach (var ext in extensions)
        {
            imageFiles.AddRange(Directory.GetFiles(wwwrootPath, ext, SearchOption.AllDirectories));
        }

        Console.WriteLine($"Toplam {imageFiles.Count} resim bulundu.\n");

        foreach (var imagePath in imageFiles)
        {
            try
            {
                string webpPath = Path.ChangeExtension(imagePath, ".webp");
                
                // Eğer webp dosyası zaten varsa ve orijinal dosyadan daha yeniyse atla
                if (File.Exists(webpPath))
                {
                    var originalTime = File.GetLastWriteTime(imagePath);
                    var webpTime = File.GetLastWriteTime(webpPath);
                    
                    if (webpTime >= originalTime)
                    {
                        Console.WriteLine($"Zaten var (atlanıyor): {Path.GetFileName(imagePath)}");
                        
                        // Orijinal dosyayı sil
                        File.Delete(imagePath);
                        convertedCount++;
                        continue;
                    }
                }

                // Resmi yükle ve WebP'ye dönüştür
                using (var image = Image.Load(imagePath))
                {
                    var encoder = new WebpEncoder
                    {
                        Quality = quality
                    };

                    image.Save(webpPath, encoder);
                }

                // Orijinal dosyayı sil
                File.Delete(imagePath);

                convertedCount++;
                Console.WriteLine($"[{convertedCount}/{imageFiles.Count}] Donusturuldu: {Path.GetFileName(imagePath)} -> {Path.GetFileName(webpPath)}");
            }
            catch (Exception ex)
            {
                errorCount++;
                Console.WriteLine($"HATA: {Path.GetFileName(imagePath)} - {ex.Message}");
            }
        }

        Console.WriteLine($"\n=== Ozet ===");
        Console.WriteLine($"Toplam dosya: {imageFiles.Count}");
        Console.WriteLine($"Basarili: {convertedCount}");
        Console.WriteLine($"Hata: {errorCount}");
    }
}
