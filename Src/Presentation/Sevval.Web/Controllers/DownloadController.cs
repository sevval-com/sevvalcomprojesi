using System.IO.Compression;
using Microsoft.AspNetCore.Mvc;

[Route("api/download")]
[ApiController]
public class DownloadController : ControllerBase
{
    [HttpPost("photos")]
    public IActionResult DownloadPhotos([FromBody] List<string> photoUrls)
    {
        using var memoryStream = new MemoryStream();
        using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
        {
            foreach (var url in photoUrls)
            {
                var fileName = Path.GetFileName(url);
                var entry = archive.CreateEntry(fileName, CompressionLevel.Fastest);
                using var entryStream = entry.Open();
                using var webClient = new System.Net.WebClient();
                var data = webClient.DownloadData(url);
                entryStream.Write(data, 0, data.Length);
            }
        }

        memoryStream.Seek(0, SeekOrigin.Begin);
        return File(memoryStream.ToArray(), "application/zip", "ilan-fotograflari.zip");
    }
}
