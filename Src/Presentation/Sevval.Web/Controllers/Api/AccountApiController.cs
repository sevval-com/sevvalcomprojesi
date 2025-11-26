using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Dtos.Email;
using Sevval.Application.Interfaces.IService;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;
using Sevval.Web.Models.Api;
using System.Security.Claims;

namespace Sevval.Web.Controllers.Api
{
    /// <summary>
    /// Mobil uygulama için Account API endpoint'leri
    /// JWT Bearer token authentication kullanır
    /// </summary>
    [ApiController]
    [Route("api/v1/account")]
    [Authorize] // JWT Bearer token authentication
    public class AccountApiController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ApplicationDbContext _context;
        private readonly IEMailService _emailService;
        private readonly ILogger<AccountApiController> _logger;
        private readonly IConfiguration _configuration;

        public AccountApiController(
            UserManager<ApplicationUser> userManager,
            ApplicationDbContext context,
            IEMailService emailService,
            ILogger<AccountApiController> logger,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _context = context;
            _emailService = emailService;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Mobil uygulama için hesap silme endpoint'i
        /// POST /api/v1/account/delete
        /// </summary>
        /// <param name="request">Silme isteği (şifre, onay metni, neden)</param>
        /// <returns>JSON response (success/error)</returns>
        [HttpPost("delete")]
        [ProducesResponseType(typeof(DeleteAccountResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(typeof(ApiErrorResponse), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteAccount([FromBody] DeleteAccountRequest request)
        {
            try
            {
                // 1. Model validasyonu
                if (!ModelState.IsValid)
                {
                    var errors = string.Join(", ", ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage));
                    
                    return BadRequest(new ApiErrorResponse
                    {
                        Success = false,
                        Message = $"Geçersiz istek: {errors}",
                        ErrorCode = "INVALID_REQUEST"
                    });
                }

                // 2. Kullanıcıyı al
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    _logger.LogWarning("DeleteAccount API: Kullanıcı bulunamadı. UserId: {UserId}", userId);
                    return Unauthorized(new ApiErrorResponse
                    {
                        Success = false,
                        Message = "Kullanıcı bulunamadı. Lütfen tekrar giriş yapın.",
                        ErrorCode = "USER_NOT_FOUND"
                    });
                }

                // 3. Şifre kontrolü
                var passwordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!passwordValid)
                {
                    _logger.LogWarning("DeleteAccount API: Yanlış şifre. UserId: {UserId}, Email: {Email}", 
                        userId, user.Email);
                    
                    return BadRequest(new ApiErrorResponse
                    {
                        Success = false,
                        Message = "Şifre hatalı. Lütfen kontrol edip tekrar deneyin.",
                        ErrorCode = "INVALID_PASSWORD"
                    });
                }

                // 4. Onay metni kontrolü
                if (request.ConfirmationText != "HESABIMI SIL")
                {
                    _logger.LogWarning("DeleteAccount API: Yanlış onay metni. UserId: {UserId}, Text: {Text}", 
                        userId, request.ConfirmationText);
                    
                    return BadRequest(new ApiErrorResponse
                    {
                        Success = false,
                        Message = "Onay metni hatalı. Lütfen tam olarak 'HESABIMI SIL' yazın (büyük harfle).",
                        ErrorCode = "INVALID_CONFIRMATION"
                    });
                }

                // 5. Hesap zaten silinmiş mi kontrol et
                var existingDeleted = await _context.DeletedAccounts
                    .FirstOrDefaultAsync(d => d.UserId == userId);

                if (existingDeleted != null)
                {
                    return BadRequest(new ApiErrorResponse
                    {
                        Success = false,
                        Message = "Hesabınız zaten silinmiş durumda.",
                        ErrorCode = "ALREADY_DELETED"
                    });
                }

                // 6. Recovery token oluştur
                var recoveryToken = Guid.NewGuid().ToString("N"); // 32 karakter hex
                var deletionDate = DateTime.UtcNow;
                var recoveryDeadline = deletionDate.AddDays(30);

                // 7. DeletedAccounts tablosuna kaydet
                var deletedAccount = new DeletedAccount
                {
                    UserId = user.Id,
                    DeletedAt = deletionDate,
                    DeletionReason = request.DeletionReason,
                    RecoveryToken = recoveryToken
                };

                await _context.DeletedAccounts.AddAsync(deletedAccount);

                // 8. AspNetUsers.IsActive güncelle
                user.IsActive = "deleted";
                await _userManager.UpdateAsync(user);

                // 9. Kullanıcının ilanlarını gizle
                var userAds = await _context.IlanBilgileri
                    .Where(i => i.Email == user.Email && i.Status != "deleted")
                    .ToListAsync();

                foreach (var ad in userAds)
                {
                    ad.Status = "deleted";
                }

                // 10. Değişiklikleri kaydet
                await _context.SaveChangesAsync();

                _logger.LogInformation(
                    "DeleteAccount API: Hesap başarıyla silindi. UserId: {UserId}, Email: {Email}, Token: {Token}",
                    userId, user.Email, recoveryToken.Substring(0, 8) + "..."
                );

                // 11. Email gönder (async, hata olsa da işlemi durdurmasın)
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _emailService.SendAccountDeletionEmailAsync(new SendAccountDeletionDto
                        {
                            ReceiverEmail = user.Email,
                            ReceiverName = $"{user.FirstName} {user.LastName}",
                            UserId = user.Id,
                            DeletionDate = deletionDate,
                            RecoveryDeadline = recoveryDeadline,
                            RecoveryToken = recoveryToken
                        });

                        _logger.LogInformation("DeleteAccount API: Email gönderildi. Email: {Email}", user.Email);
                    }
                    catch (Exception emailEx)
                    {
                        _logger.LogError(emailEx, "DeleteAccount API: Email gönderimi başarısız. Email: {Email}", user.Email);
                    }
                });

                // 12. Başarılı response döndür
                return Ok(new DeleteAccountResponse
                {
                    Success = true,
                    Message = "Hesabınız başarıyla silindi. 30 gün içinde email'deki link ile veya tekrar giriş yaparak hesabınızı kurtarabilirsiniz.",
                    DeletionDate = deletionDate,
                    RecoveryDeadline = recoveryDeadline,
                    RecoveryToken = recoveryToken
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "DeleteAccount API: Beklenmeyen hata. UserId: {UserId}", 
                    User.FindFirstValue(ClaimTypes.NameIdentifier));
                
                return StatusCode(500, new ApiErrorResponse
                {
                    Success = false,
                    Message = "Beklenmeyen bir hata oluştu. Lütfen tekrar deneyin.",
                    ErrorCode = "INTERNAL_ERROR"
                });
            }
        }

        /// <summary>
        /// Hesap durumu kontrolü endpoint'i
        /// GET /api/v1/account/status
        /// </summary>
        [HttpGet("status")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> GetAccountStatus()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return Unauthorized(new ApiErrorResponse
                    {
                        Success = false,
                        Message = "Kullanıcı bulunamadı",
                        ErrorCode = "USER_NOT_FOUND"
                    });
                }

                // Silinen hesap kontrolü
                var deletedAccount = await _context.DeletedAccounts
                    .FirstOrDefaultAsync(d => d.UserId == userId);

                if (deletedAccount != null)
                {
                    var daysUntilDeletion = 30 - (DateTime.UtcNow - deletedAccount.DeletedAt).Days;

                    return Ok(new
                    {
                        success = true,
                        isDeleted = true,
                        isActive = user.IsActive,
                        deletedAt = deletedAccount.DeletedAt,
                        recoveryDeadline = deletedAccount.DeletedAt.AddDays(30),
                        daysUntilPermanentDeletion = daysUntilDeletion > 0 ? daysUntilDeletion : 0
                    });
                }

                return Ok(new
                {
                    success = true,
                    isDeleted = false,
                    isActive = user.IsActive,
                    deletedAt = (DateTime?)null,
                    recoveryDeadline = (DateTime?)null,
                    daysUntilPermanentDeletion = (int?)null
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetAccountStatus API error");
                return StatusCode(500, new ApiErrorResponse
                {
                    Success = false,
                    Message = "Hata oluştu",
                    ErrorCode = "INTERNAL_ERROR"
                });
            }
        }

        /// <summary>
        /// API endpoint test için health check
        /// GET /api/v1/account/health
        /// </summary>
        [HttpGet("health")]
        [AllowAnonymous]
        public IActionResult HealthCheck()
        {
            return Ok(new
            {
                status = "healthy",
                timestamp = DateTime.UtcNow,
                version = "1.0",
                endpoints = new[]
                {
                    "POST /api/v1/account/delete",
                    "GET /api/v1/account/status",
                    "GET /api/v1/account/health"
                }
            });
        }
    }
}
