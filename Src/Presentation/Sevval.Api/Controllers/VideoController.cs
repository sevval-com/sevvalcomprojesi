using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Features.Video.Queries.GetVideos;
using Sevval.Domain.Entities;
using Sevval.Domain.Enums;
using Sevval.Persistence.Context;
using Sevval.Web.Models;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Sevval.Api.Controllers
{
    [Route("api/v1/videos")]
    [ApiController]
    public class VideoController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public VideoController(IMediator mediator, ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _mediator = mediator;
            _context = context;
            _userManager = userManager;
        }

        private static string BuildAbsoluteUrl(string baseUrl, string? path)
        {
            if (string.IsNullOrWhiteSpace(path)) return null;
            if (path.StartsWith("http://") || path.StartsWith("https://")) return path;
            return path.StartsWith("/") ? baseUrl + path : baseUrl + "/" + path;
        }

        /// <summary>
        /// Videoları getirir
        /// </summary>
        /// <param name="request">Video filtreleme parametreleri</param>
        /// <returns>Video listesi ve kategori bilgileri</returns>
        [HttpGet]
        public async Task<IActionResult> GetVideos([FromQuery] string? category = null)
        {
            // Videoları doğrudan VideolarSayfasi tablosundan oku ve basit DTO'ya dönüştür
            // Sadece onaylanmış videoları getir
            var query = _context.VideolarSayfasi.Where(v => v.ApprovalStatus == VideoApprovalStatus.Approved).AsQueryable();
            if (!string.IsNullOrWhiteSpace(category))
            {
                query = query.Where(v => v.Kategori == category);
            }

            var baseUrl = $"{Request.Scheme}://{Request.Host.ToUriComponent()}";

            var rows = await query
                .OrderByDescending(v => v.YuklenmeTarihi)
                .Select(v => new
                {
                    v.Id,
                    v.VideoAdi,
                    v.VideoAciklamasi,
                    v.VideoYolu,
                    v.KapakFotografiYolu,
                    v.IsYouTube,
                    v.Kategori,
                    v.YuklenmeTarihi,
                    v.GoruntulenmeSayisi
                })
                .ToListAsync();

            var list = rows.Select(v => new
            {
                id = v.Id,
                title = v.VideoAdi,
                description = v.VideoAciklamasi,
                videoUrl = v.IsYouTube ? ("https://www.youtube.com/embed/" + v.VideoYolu) : BuildAbsoluteUrl(baseUrl, v.VideoYolu),
                thumbnailUrl = !string.IsNullOrEmpty(v.KapakFotografiYolu)
                    ? BuildAbsoluteUrl(baseUrl, v.KapakFotografiYolu)
                    : (v.IsYouTube && !string.IsNullOrEmpty(v.VideoYolu)
                        ? ($"https://img.youtube.com/vi/{v.VideoYolu}/hqdefault.jpg")
                        : null),
                category = v.Kategori,
                isActive = true,
                createdDate = v.YuklenmeTarihi,
                viewCount = v.GoruntulenmeSayisi,
                duration = 0
            }).ToList();

            return Ok(new
            {
                isSuccessfull = true,
                message = "Videolar basariyla getirildi.",
                code = 0,
                data = list,
                meta = (object?)null,
                errors = Array.Empty<object>()
            });
        }

        /// <summary>
        /// Video detaylarını getirir ve görüntülenme sayısını artırır
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <returns>Video detayları, yorumlar ve öneri videoları</returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetVideoDetails(int id)
        {
            var video = await _context.VideolarSayfasi
                .Include(v => v.YukleyenKullanici)
                .FirstOrDefaultAsync(v => v.Id == id);

            if (video == null)
            {
                return NotFound(new { message = "Video bulunamadı" });
            }

            // Onaylanmamış videoya erişimi engelle (video sahibi veya admin hariç)
            if (video.ApprovalStatus != VideoApprovalStatus.Approved)
            {
                return NotFound(new { message = "Video bulunamadı veya henüz onaylanmamış" });
            }

            // Görüntülenme sayısını artır
            video.GoruntulenmeSayisi++;
            _context.Update(video);
            await _context.SaveChangesAsync();

            // Öneri videoları - sadece onaylanmış olanlar
            var oneriVideolar = await _context.VideolarSayfasi
                .Where(v => v.Kategori == video.Kategori && v.Id != id && v.ApprovalStatus == VideoApprovalStatus.Approved)
                .Include(v => v.YukleyenKullanici)
                .OrderByDescending(v => v.YuklenmeTarihi)
                .Take(10)
                .Select(v => new
                {
                    v.Id,
                    v.VideoAdi,
                    v.KapakFotografiYolu,
                    v.VideoYolu,
                    v.IsYouTube,
                    v.GoruntulenmeSayisi,
                    v.YuklenmeTarihi,
                    v.Kategori,
                    YukleyenKullanici = new
                    {
                        v.YukleyenKullanici.Id,
                        v.YukleyenKullanici.FirstName,
                        v.YukleyenKullanici.LastName,
                        v.YukleyenKullanici.ProfilePicturePath
                    }
                })
                .ToListAsync();

            // Yorumlar
            var yorumlar = await _context.VideoYorumlari
                .Where(c => c.VideoId == id && c.ParentYorumId == null)
                .Include(c => c.Kullanici)
                .Include(c => c.Yanitlar)
                    .ThenInclude(r => r.Kullanici)
                .OrderByDescending(c => c.YorumTarihi)
                .Select(c => new
                {
                    c.Id,
                    c.YorumMetni,
                    c.YorumTarihi,
                    Kullanici = new
                    {
                        c.Kullanici.Id,
                        c.Kullanici.FirstName,
                        c.Kullanici.LastName,
                        c.Kullanici.ProfilePicturePath
                    },
                    Yanitlar = c.Yanitlar.OrderBy(r => r.YorumTarihi).Select(r => new
                    {
                        r.Id,
                        r.YorumMetni,
                        r.YorumTarihi,
                        Kullanici = new
                        {
                            r.Kullanici.Id,
                            r.Kullanici.FirstName,
                            r.Kullanici.LastName,
                            r.Kullanici.ProfilePicturePath
                        }
                    })
                })
                .ToListAsync();

            // Kullanıcının oyunu
            bool? currentUserVote = null;
            if (User.Identity.IsAuthenticated)
            {
                var userId = _userManager.GetUserId(User);
                var vote = await _context.VideoLikes.FirstOrDefaultAsync(l => l.VideoId == id && l.UserId == userId);
                currentUserVote = vote?.IsLike;

                // İzlenen video olarak işaretle
                var existingWatch = await _context.VideoWatches.FirstOrDefaultAsync(w => w.VideoId == id && w.UserId == userId);
                if (existingWatch == null)
                {
                    _context.VideoWatches.Add(new VideoWatch { VideoId = id, UserId = userId, WatchedAtUtc = DateTime.UtcNow });
                    await _context.SaveChangesAsync();
                }
            }

            var result = new
            {
                Video = new
                {
                    video.Id,
                    video.VideoAdi,
                    video.VideoAciklamasi,
                    video.VideoYolu,
                    video.KapakFotografiYolu,
                    video.IsYouTube,
                    video.Kategori,
                    video.GoruntulenmeSayisi,
                    video.BegeniSayisi,
                    video.DislikeSayisi,
                    video.YuklenmeTarihi,
                    YukleyenKullanici = new
                    {
                        video.YukleyenKullanici.Id,
                        video.YukleyenKullanici.FirstName,
                        video.YukleyenKullanici.LastName,
                        video.YukleyenKullanici.ProfilePicturePath
                    }
                },
                OneriVideolar = oneriVideolar,
                Yorumlar = yorumlar,
                CurrentUserVote = currentUserVote
            };

            return Ok(result);
        }

        /// <summary>
        /// Video beğeni/beğenmeme işlemi (Login gerektirmez - IP ile takip)
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <param name="request">Beğeni bilgisi</param>
        /// <returns>Güncellenmiş beğeni sayıları</returns>
        [HttpPost("{id}/vote")]
        public async Task<IActionResult> VoteVideo(int id, [FromBody] VoteRequest request)
        {
            try
            {
                var video = await _context.VideolarSayfasi.FindAsync(id);
                if (video == null) return NotFound(new { message = "Video bulunamadı" });

                var clientIp = !string.IsNullOrEmpty(request.IpAddress) ? request.IpAddress : GetClientIpAddress();
                var userId = User.Identity.IsAuthenticated ? _userManager.GetUserId(User) : null;

            // Aynı IP'den daha önce oy verilmiş mi kontrol et
            var existingVote = await _context.VideoLikes.FirstOrDefaultAsync(l => 
                l.VideoId == id && 
                ((userId != null && l.UserId == userId) || (userId == null && l.IpAddress == clientIp)));

            bool? voteStatus = null;

            if (existingVote != null)
            {
                if (existingVote.IsLike == request.IsLike)
                {
                    // Aynı oyu geri al
                    _context.VideoLikes.Remove(existingVote);
                    if (request.IsLike) video.BegeniSayisi--; else video.DislikeSayisi--;
                    voteStatus = null;
                }
                else
                {
                    // Oyunu değiştir
                    if (request.IsLike) { video.BegeniSayisi++; video.DislikeSayisi--; }
                    else { video.BegeniSayisi--; video.DislikeSayisi++; }
                    existingVote.IsLike = request.IsLike;
                    voteStatus = request.IsLike;
                }
            }
            else
            {
                // Yeni oy
                var newLike = new VideoLike 
                { 
                    VideoId = id, 
                    UserId = userId ?? "", // Null ise boş string kullan
                    IpAddress = userId == null ? clientIp : null,
                    IsLike = request.IsLike 
                };
                _context.VideoLikes.Add(newLike);
                if (request.IsLike) video.BegeniSayisi++; else video.DislikeSayisi++;
                voteStatus = request.IsLike;
            }

                await _context.SaveChangesAsync();
                return Ok(new
                {
                    success = true,
                    likeCount = video.BegeniSayisi,
                    dislikeCount = video.DislikeSayisi,
                    voteStatus
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { 
                    success = false, 
                    message = "Sunucu hatası: " + ex.Message,
                    details = ex.InnerException?.Message
                });
            }
        }

        /// <summary>
        /// Test endpoint
        /// </summary>
        /// <returns>Test mesajı</returns>
        [HttpGet("test")]
        public IActionResult Test()
        {
            return Ok(new { message = "API çalışıyor!", timestamp = DateTime.UtcNow });
        }

        /// <summary>
        /// Önerilen videoları getir (aynı kategori veya en son yüklenenler)
        /// </summary>
        /// <param name="currentVideoId">Mevcut video ID</param>
        /// <param name="category">Video kategorisi</param>
        /// <param name="limit">Maksimum video sayısı</param>
        /// <returns>Önerilen videolar listesi</returns>
        [HttpGet("suggested")]
        public async Task<IActionResult> GetSuggestedVideos(int currentVideoId, string? category = null, int limit = 6)
        {
            try
            {
                // Debug: Gelen kategori değerini logla
                Console.WriteLine($"Gelen kategori: '{category}'");
                var query = _context.VideolarSayfasi
                    .Include(v => v.YukleyenKullanici)
                    .Where(v => v.Id != currentVideoId && v.ApprovalStatus == VideoApprovalStatus.Approved) // Mevcut videoyu hariç tut ve sadece onaylı videoları getir
                    .AsQueryable();

                // Not: Kategori belirtilse bile hard filter yapmayalım; böylece aynı kategori yoksa
                // en son yüklenen videolar otomatik olarak fallback olur. Aynı kategori önceliğini
                // sıralamada vereceğiz.

                // Önce aynı kategorideki videoları, sonra en son yüklenenleri getir
                var normalizedCategoryForSort = !string.IsNullOrEmpty(category) ? category.Trim().TrimEnd('.') : "";
                var suggestedVideos = await query
                    .OrderByDescending(v => !string.IsNullOrEmpty(normalizedCategoryForSort) && v.Kategori.Trim().TrimEnd('.') == normalizedCategoryForSort ? 1 : 0) // Aynı kategori öncelikli
                    .ThenByDescending(v => v.YuklenmeTarihi)
                    .Take(limit)
                    .Select(v => new
                    {
                        v.Id,
                        v.VideoAdi,
                        v.KapakFotografiYolu,
                        v.VideoYolu,
                        v.IsYouTube,
                        v.Kategori,
                        v.GoruntulenmeSayisi,
                        v.BegeniSayisi,
                        v.DislikeSayisi,
                        v.YuklenmeTarihi,
                        YukleyenKullanici = new
                        {
                            v.YukleyenKullanici.Id,
                            v.YukleyenKullanici.FirstName,
                            v.YukleyenKullanici.LastName,
                            v.YukleyenKullanici.ProfilePicturePath
                        }
                    })
                    .ToListAsync();

                return Ok(new
                {
                    success = true,
                    videos = suggestedVideos
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = "Önerilen videolar getirilemedi: " + ex.Message
                });
            }
        }

        /// <summary>
        /// Client IP adresini al
        /// </summary>
        /// <returns>Client IP adresi</returns>
        private string GetClientIpAddress()
        {
            var httpContext = HttpContext;
            
            // X-Forwarded-For header'ını kontrol et (proxy/load balancer için)
            var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
            if (!string.IsNullOrEmpty(forwardedFor))
            {
                var ips = forwardedFor.Split(',');
                if (ips.Length > 0)
                {
                    return ips[0].Trim();
                }
            }

            // X-Real-IP header'ını kontrol et
            var realIp = httpContext.Request.Headers["X-Real-IP"].FirstOrDefault();
            if (!string.IsNullOrEmpty(realIp))
            {
                return realIp;
            }

            // RemoteIpAddress'i kullan
            return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
        }

        /// <summary>
        /// Video yorumu ekle
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <param name="request">Yorum bilgileri</param>
        /// <returns>Eklenen yorum bilgileri</returns>
        [HttpPost("{id}/comments")]
        [Authorize]
        public async Task<IActionResult> AddComment(int id, [FromBody] AddCommentRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.YorumMetni) || request.YorumMetni.Length > 1000)
            {
                return BadRequest(new { message = "Yorum metni geçersiz." });
            }

            var user = await _userManager.GetUserAsync(User);
            var video = await _context.VideolarSayfasi.FindAsync(id);
            if (user == null || video == null)
            {
                return BadRequest(new { message = "Geçersiz istek." });
            }

            if (request.ParentYorumId.HasValue)
            {
                var parentYorum = await _context.VideoYorumlari.FindAsync(request.ParentYorumId.Value);
                if (parentYorum == null || parentYorum.VideoId != id)
                {
                    return BadRequest(new { message = "Geçersiz ana yorum." });
                }
            }

            var yeniYorum = new VideoYorum
            {
                VideoId = id,
                YorumMetni = request.YorumMetni,
                UserId = user.Id,
                YorumTarihi = DateTime.UtcNow,
                ParentYorumId = request.ParentYorumId
            };

            _context.VideoYorumlari.Add(yeniYorum);
            await _context.SaveChangesAsync();

            var isVideoOwner = video.YukleyenKullaniciId == user.Id;

            return Ok(new
            {
                success = true,
                yorumId = yeniYorum.Id,
                parentYorumId = yeniYorum.ParentYorumId,
                yorumMetni = yeniYorum.YorumMetni,
                yorumTarihi = yeniYorum.YorumTarihi,
                kullanici = new
                {
                    id = user.Id,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    profilePicturePath = user.ProfilePicturePath
                },
                isVideoOwner
            });
        }

        /// <summary>
        /// Video yorumu sil (sadece sftumen41@gmail.com)
        /// </summary>
        /// <param name="commentId">Yorum ID</param>
        /// <returns>Silme sonucu</returns>
        [HttpDelete("comments/{commentId}")]
        [Authorize]
        public async Task<IActionResult> DeleteComment(int commentId)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !string.Equals(user.Email, "sftumen41@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var comment = await _context.VideoYorumlari
                .Include(c => c.Yanitlar)
                .FirstOrDefaultAsync(c => c.Id == commentId);
            if (comment == null)
            {
                return NotFound(new { message = "Yorum bulunamadı." });
            }

            // Önce yanıtları sil, sonra ana yorumu sil
            if (comment.Yanitlar != null && comment.Yanitlar.Count > 0)
            {
                _context.VideoYorumlari.RemoveRange(comment.Yanitlar);
            }
            _context.VideoYorumlari.Remove(comment);
            await _context.SaveChangesAsync();

            return Ok(new { success = true });
        }

        /// <summary>
        /// Kullanıcının izlediği videoları getirir
        /// </summary>
        /// <returns>İzlenen video ID'leri ve detayları</returns>
        [HttpGet("watched")]
        [Authorize]
        public async Task<IActionResult> GetWatchedVideos()
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var watchedVideos = await (from w in _context.VideoWatches
                                       join v in _context.VideolarSayfasi on w.VideoId equals v.Id
                                       join u in _context.Users on v.YukleyenKullaniciId equals u.Id
                                       where w.UserId == userId
                                       orderby w.WatchedAtUtc descending
                                       select new
                                       {
                                           VideoId = w.VideoId,
                                           WatchedAt = w.WatchedAtUtc,
                                           Video = new
                                           {
                                               v.Id,
                                               v.VideoAdi,
                                               v.KapakFotografiYolu,
                                               v.VideoYolu,
                                               v.IsYouTube,
                                               v.Kategori,
                                               v.GoruntulenmeSayisi,
                                               v.BegeniSayisi,
                                               v.DislikeSayisi,
                                               v.YuklenmeTarihi,
                                               YukleyenKullanici = new
                                               {
                                                   u.Id,
                                                   u.FirstName,
                                                   u.LastName,
                                                   u.ProfilePicturePath
                                               }
                                           }
                                       }).ToListAsync();

            return Ok(new 
            { 
                watchedVideos,
                totalCount = watchedVideos.Count,
                watchedIds = watchedVideos.Select(w => w.VideoId).ToList()
            });
        }

        /// <summary>
        /// Videoyu izlendi olarak işaretle
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <returns>İşaretleme sonucu</returns>
        [HttpPost("{id}/watch")]
        [Authorize]
        public async Task<IActionResult> MarkVideoAsWatched(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var video = await _context.VideolarSayfasi.FindAsync(id);
            if (video == null)
            {
                return NotFound(new { message = "Video bulunamadı" });
            }

            var existingWatch = await _context.VideoWatches
                .FirstOrDefaultAsync(w => w.VideoId == id && w.UserId == userId);

            if (existingWatch == null)
            {
                _context.VideoWatches.Add(new VideoWatch
                {
                    VideoId = id,
                    UserId = userId,
                    WatchedAtUtc = DateTime.UtcNow
                });
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    success = true, 
                    message = "Video izlendi olarak işaretlendi",
                    watchedAt = DateTime.UtcNow
                });
            }
            else
            {
                return Ok(new 
                { 
                    success = true, 
                    message = "Video zaten izlendi olarak işaretli",
                    watchedAt = existingWatch.WatchedAtUtc
                });
            }
        }

        /// <summary>
        /// Videoyu izlenmedi olarak işaretle (izlendi işaretini kaldır)
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <returns>İşaretleme kaldırma sonucu</returns>
        [HttpDelete("{id}/watch")]
        [Authorize]
        public async Task<IActionResult> UnmarkVideoAsWatched(int id)
        {
            var userId = _userManager.GetUserId(User);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            var existingWatch = await _context.VideoWatches
                .FirstOrDefaultAsync(w => w.VideoId == id && w.UserId == userId);

            if (existingWatch != null)
            {
                _context.VideoWatches.Remove(existingWatch);
                await _context.SaveChangesAsync();

                return Ok(new 
                { 
                    success = true, 
                    message = "Video izlendi işareti kaldırıldı"
                });
            }
            else
            {
                return NotFound(new 
                { 
                    success = false, 
                    message = "Video zaten izlenmedi olarak işaretli"
                });
            }
        }

        /// <summary>
        /// Video silme (sadece sftumen41@gmail.com)
        /// </summary>
        /// <param name="id">Video ID</param>
        /// <returns>Silme sonucu</returns>
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteVideo(int id)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !string.Equals(user.Email, "sftumen41@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            var video = await _context.VideolarSayfasi.FirstOrDefaultAsync(v => v.Id == id);
            if (video == null)
            {
                return NotFound(new { message = "Video bulunamadı" });
            }

            // İlgili yorumları ve beğenileri sil
            var comments = await _context.VideoYorumlari.Where(c => c.VideoId == id).ToListAsync();
            if (comments.Any())
            {
                _context.VideoYorumlari.RemoveRange(comments);
            }

            var likes = await _context.VideoLikes.Where(l => l.VideoId == id).ToListAsync();
            if (likes.Any())
            {
                _context.VideoLikes.RemoveRange(likes);
            }

            var watches = await _context.VideoWatches.Where(w => w.VideoId == id).ToListAsync();
            if (watches.Any())
            {
                _context.VideoWatches.RemoveRange(watches);
            }

            // Videoyu sil
            _context.VideolarSayfasi.Remove(video);
            await _context.SaveChangesAsync();

            return Ok(new { success = true, message = "Video başarıyla silindi" });
        }

        /// <summary>
        /// Video yükleme (sadece sftumen41@gmail.com)
        /// </summary>
        /// <param name="request">Video yükleme bilgileri</param>
        /// <returns>Yükleme sonucu</returns>
        [HttpPost("upload")]
        [Authorize]
        public async Task<IActionResult> UploadVideo([FromForm] VideoUploadApiRequest request)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null || !string.Equals(user.Email, "sftumen41@gmail.com", StringComparison.OrdinalIgnoreCase))
            {
                return Forbid();
            }

            if (request.VideoDosyasi == null && string.IsNullOrEmpty(request.YouTubeLink))
            {
                return BadRequest(new { message = "Lütfen bir video dosyası yükleyin veya bir YouTube linki girin." });
            }

            var yeniVideo = new VideolarSayfasi
            {
                VideoAdi = request.VideoAdi,
                VideoAciklamasi = request.VideoAciklamasi,
                Kategori = request.Kategori,
                YukleyenKullaniciId = user.Id,
                YuklenmeTarihi = DateTime.UtcNow
            };

            if (!string.IsNullOrEmpty(request.YouTubeLink))
            {
                yeniVideo.IsYouTube = true;
                yeniVideo.VideoYolu = ExtractYouTubeId(request.YouTubeLink);
                yeniVideo.KapakFotografiYolu = $"https://img.youtube.com/vi/{yeniVideo.VideoYolu}/hqdefault.jpg";
            }

            _context.VideolarSayfasi.Add(yeniVideo);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                success = true,
                videoId = yeniVideo.Id,
                message = "Video başarıyla yüklendi"
            });
        }

        private string ExtractYouTubeId(string url)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"(?:https?:\/\/)?(?:www\.)?(?:(?:youtube\.com\/watch\?[^?]*v=|youtu\.be\/)([\w\-]+))(?:[^\s?&]*)", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
            var match = regex.Match(url);
            return match.Success ? match.Groups[1].Value : null;
        }
    }

    // API Request Models
    public class VoteRequest
    {
        public bool IsLike { get; set; }
        public string? IpAddress { get; set; }
    }

    public class AddCommentRequest
    {
        public string YorumMetni { get; set; }
        public int? ParentYorumId { get; set; }
    }

    public class VideoUploadApiRequest
    {
        public string VideoAdi { get; set; }
        public string VideoAciklamasi { get; set; }
        public string Kategori { get; set; }
        public IFormFile VideoDosyasi { get; set; }
        public string YouTubeLink { get; set; }
        public IFormFile KapakFotografi { get; set; }
    }
}
