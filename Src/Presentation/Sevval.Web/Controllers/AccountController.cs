using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualBasic;
using Sevval.Application.Constants;
using Sevval.Application.Dtos.Email;
using Sevval.Application.Features.User.Commands.ConfirmEstate;
using Sevval.Application.Features.User.Commands.CorporateRegister;
using Sevval.Application.Features.User.Commands.ForgottenPassword;
using Sevval.Application.Features.User.Commands.IndividualRegister;
using Sevval.Application.Features.User.Commands.LoginWithSocialMedia;
using Sevval.Application.Features.User.Commands.RejectEstate;
using Sevval.Application.Features.User.Commands.SendNewCode;
using Sevval.Application.Interfaces.IService;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;
using sevvalemlak.csproj.ClientServices.UserServices;
using sevvalemlak.csproj.Dto.Account;
using sevvalemlak.csproj.Helpers;
using sevvalemlak.Models;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web; // HttpUtility için

public class AccountController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly IEMailService _emailService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<AccountController> _logger;
    // Doğrulama kodları ve emlak kayıt bilgileri için statik sözlükler
    private static readonly Dictionary<string, (string Code, DateTime Expiry)> _verificationStorage = new();
    private static readonly Dictionary<string, (EstateRegisterInfoModel model, DateTime Expiry)> _verifyEstateStorage = new();
    private const string SecretKey = "SeVVAlEmlaK3300129231321943198419SEVVALEMLAK";
    private readonly ApplicationDbContext _context;

    private IUserClientService _userClientService;

    // Constructor: Bağımlılık Enjeksiyonu ile gerekli servisleri alır
    public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager, IEMailService emailService, IConfiguration configuration, ApplicationDbContext context, ILogger<AccountController> logger, IUserClientService userClientService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _emailService = emailService;
        _configuration = configuration;
        _context = context;
        _logger = logger;
        _userClientService = userClientService;
    }


    #region Tamamlananlar

    // Kullanıcı giriş işlemini yönetir
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {

            // E-posta veya telefon numarasına göre kullanıcı ara
            var user = await _userManager.FindByEmailAsync(model.Email) ?? await _userManager.Users.FirstOrDefaultAsync(u => u.PhoneNumber == model.Email);

            if (user != null)
            {
                // 🆕 PASSIVE ACCOUNT CHECK - Admin onayı bekleyen kullanıcılar giriş yapamaz
                if (user.IsActive == "passive")
                {
                    ModelState.AddModelError(string.Empty, 
                        "Hesabınız henüz onaylanmamış. Lütfen yönetici onayını bekleyiniz. " +
                        "Onay sonrası e-posta ile bilgilendirileceksiniz.");
                    _logger.LogWarning("Login engellendi: Kullanıcı ({Email}) passive durumda", model.Email);
                    return View(model);
                }
                
                // CHECK FOR DELETED ACCOUNT RECOVERY (30-day window)
                // Handle ANY non-active account that has a DeletedAccounts record
                if (user.IsActive != "active")
                {
                    // Check if account was deleted (has DeletedAccounts record)
                    DeletedAccount? deletedAccount = null;
                    try
                    {
                        deletedAccount = await _context.DeletedAccounts
                            .FirstOrDefaultAsync(d => d.UserId == user.Id);
                    }
                    catch (Exception)
                    {
                        // DeletedAccounts tablosu henüz oluşturulmamış, devam et
                    }
                    
                    if (deletedAccount != null)
                    {
                        // Account was deleted - check if within recovery window
                        var daysSinceDeletion = (DateTime.UtcNow - deletedAccount.DeletedAt).TotalDays;
                        
                        if (daysSinceDeletion <= 30)
                        {
                            // RESTORE ACCOUNT - within recovery window
                            user.IsActive = "active";
                            await _userManager.UpdateAsync(user);
                            
                            // Restore user's IlanBilgileri
                            var userAds = await _context.IlanBilgileri
                                .Where(i => i.Email == user.Email && i.Status == "deleted")
                                .ToListAsync();
                            
                            foreach (var ad in userAds)
                            {
                                ad.Status = "active";
                            }
                            
                            // Remove from DeletedAccounts table
                            _context.DeletedAccounts.Remove(deletedAccount);
                            await _context.SaveChangesAsync();
                            
                            // Sign in user and show success message
                            await _signInManager.SignInAsync(user, isPersistent: true);
                            TempData["SuccessMessage"] = "Hoş geldiniz! Hesabınız başarıyla kurtarıldı ve tekrar aktif edildi.";
                            return RedirectToAction("Index", "Home");
                        }
                        else
                        {
                            // Beyond 30 days - account cannot be recovered
                            ModelState.AddModelError(string.Empty, 
                                "Hesabınız 30 günlük kurtarma süresini aştığı için kalıcı olarak silinmiştir. Yeni bir hesap oluşturabilirsiniz.");
                            return View(model);
                        }
                    }
                    else
                    {
                        // Not deleted, just inactive - show original error
                        ModelState.AddModelError(string.Empty, "Hesabınız aktif edilmemiş. Lütfen daha sonra tekrar deneyin.");
                        return View(model);
                    }
                }

                if (user.EmailConfirmed == false)
                {
                    ModelState.AddModelError(string.Empty, "Hesabınız onaylanmamış. Lütfen e-posta adresinizi onaylayın.");
                    return View(model);
                }

                if (user.EmailConfirmed) // Kurumsal girişler için e-posta onayı zorunlu değilse bu kısım değişebilir
                {
                    var result = await _signInManager.PasswordSignInAsync(user, model.Password, isPersistent: true, lockoutOnFailure: false);
                    if (result.Succeeded)
                        return RedirectToAction("Index", "Home");
                    else if (result.IsLockedOut)
                        ModelState.AddModelError(string.Empty, "Hesabınız kilitlendi. Lütfen daha sonra tekrar deneyin.");
                    else if (result.RequiresTwoFactor)
                        return RedirectToAction("TwoFactorVerification", new { ReturnUrl = Url.Action("Index", "Home") }); // İki faktörlü kimlik doğrulama
                    else
                        ModelState.AddModelError(string.Empty, "Geçersiz şifre.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "E-posta adresiniz doğrulanmamış. Lütfen e-postanızı kontrol edin.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı bulunamadı.");
            }
        }


        return View(model);
    }

    public IActionResult MailOnay()
    {
        return View();
    }

    // Emlakçı kaydını onaylar
    [HttpGet("ConfirmEstate")]
    public async Task<IActionResult> ConfirmEstate([FromQuery] string email, [FromQuery] string code)
    {

        var response = await _userClientService.ConfirmEstate(new ConfirmEstateCommandRequest { Email = email, Token = code }, CancellationToken.None);

        if (response.IsSuccessfull)
        {
            _logger.LogInformation("Emlakçı hesabı başarıyla onaylandı ve oluşturuldu: {Email}", email);

            return View(); // Başarılı onay sayfası
        }
        else
        {
            _logger.LogError(response.Message, string.Join(", ", response.Errors.Select(e => e.Message)));

            return BadRequest("Kullanıcı doğrulanırken bir sorun oluştu.");
        }

    }

    // Emlakçı kaydını reddeder
    [HttpGet("RejectEstate")]
    public async Task<IActionResult> RejectEstate([FromQuery] string email, [FromQuery] string code)
    {
        var response = await _userClientService.RejectEstate(new RejectEstateCommandRequest { Email = email, Token = code }, CancellationToken.None);

        if (response.IsSuccessfull)
        {
            _logger.LogInformation("Emlakçı kaydı reddedildi: {Email}", email);

            return View(); // Reddedildi sayfası
        }
        else
        {
            return BadRequest(response.Message);

        }

    }

    // E-posta adresini doğrular  bunun apisini yazmaya gerek yok
    [HttpGet("VerifyEmail")]
    public async Task<IActionResult> VerifyEmail([FromQuery] string email, [FromQuery] string code)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                return Ok("Eposta Adresi geçerli değil.");
            }

            string urlDecoded = HttpUtility.UrlDecode(code);


            string decodedCode = Md5Service.Decrypt(urlDecoded);

            var res = await _userManager.ConfirmEmailAsync(user, decodedCode);

            if (res.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: true);
                _logger.LogInformation("E-posta başarıyla doğrulandı ve kullanıcı giriş yaptı: {Email}", user.Email);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                _logger.LogError("E-posta doğrulanırken hata oluştu: {Errors}", string.Join(", ", res.Errors.Select(e => e.Description)));
                return Ok("Mail Adresiniz doğrulanırken bir sorun oluştu. Geliştiriciler ile iletişime geçiniz.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "E-posta doğrulama sırasında hata oluştu. E-posta: {Email}, Kod: {Code}", email, code);
            return StatusCode(500, "Beklenmeyen bir hata oluştu.");
        }
    }


    [HttpGet]
    public IActionResult SendNewCode()
    {
        return View(new SendNewCodeDto());
    }

    // Yeni doğrulama kodu gönderir
    [HttpPost]
    public async Task<IActionResult> SendNewCode(SendNewCodeDto model)
    {
        if (!ModelState.IsValid)
        {
            ModelState.AddModelError(string.Empty, "E-posta adresi boş olamaz.");
            return View(model);
        }

        var response = await _userClientService.SendNewCode(new SendNewCodeCommandRequest { Email = model.Email }, CancellationToken.None);


        if (!response.IsSuccessfull)
        {
            ModelState.AddModelError(string.Empty, response.Message);
            return View("SendNewCode", new SendNewCodeDto());

        }

        return RedirectToAction("ForgotPassword", new ForgotPasswordViewModel() { Email = model.Email });
    }


    [HttpGet]
    public IActionResult ForgotPassword(string email)
    {
        return View(new ForgotPasswordViewModel() { Email = email });
    }

    // Şifre sıfırlama işlemini yönetir (doğrulama kodu ile)
    [HttpPost]
    public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {

            var response = await _userClientService.ForgottenPassword(new ForgottenPasswordCommandRequest
            {
                Email = model.Email,
                Code = model.Code,
                NewPassword = model.NewPassword
            }, CancellationToken.None);


            if (response.IsSuccessfull)
            {
                _logger.LogInformation("Kullanıcı ({Email}) şifresini başarıyla sıfırladı.", model.Email);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, response.Message);

        }
        return View(model);
    }

    // Kullanıcı çıkışını yönetir
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        _logger.LogInformation("Kullanıcı ({UserName}) çıkış yaptı.", User.Identity?.Name);
        return RedirectToAction("Index", "Home");
    }


    [HttpGet]
    public IActionResult Register()
    {
        return View();
    }

    public IActionResult EstateRegister()
    {
        return View();
    }

    [HttpGet]
    public IActionResult Login()
    {
        return View();
    }

    [HttpGet]
    public IActionResult CorporateLogin()
    {
        return View();
    }

    // E-posta veya telefon numarasının zaten kullanımda olup olmadığını kontrol eder
    [HttpPost]
    public async Task<JsonResult> CheckUserExists(string? email, string? phone)
    {
        bool emailExists = false;
        bool phoneExists = false;

        if (!string.IsNullOrWhiteSpace(email))
        {
            var normalizedEmail = email.ToUpperInvariant(); // Normalize edilmiş e-posta
            emailExists = await _context.Users.AsNoTracking().AnyAsync(u => u.NormalizedEmail == normalizedEmail);
        }

        if (!string.IsNullOrWhiteSpace(phone))
        {
            phoneExists = await _context.Users.AsNoTracking().AnyAsync(u => u.PhoneNumber == phone);
        }

        return Json(new { emailExists, phoneExists });
    }

    [HttpGet]
    public IActionResult SendEstateVerifyInfo()
    {
        return View();
    }


    // Kullanıcı kayıt işlemini yönetir
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            // ✅ KRİTİK GÜVENLİK: Unique email kontrolü
            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                ModelState.AddModelError("Email", "Bu e-posta adresi sistemde zaten kayıtlı. Lütfen giriş yapınız veya farklı bir e-posta kullanınız.");
                _logger.LogWarning("Register: Email already exists - {Email}", model.Email);
                return View(model);
            }

            var response = await _userClientService.IndividualRegister(new IndividualRegisterCommandRequest
            {
                Email = model.Email,
                Password = model.Password,
                FirstName = model.FirstName,
                LastName = model.LastName,
                PhoneNumber = model.PhoneNumber,
                ProfilePicture = model.ProfilePicture,
                ConfirmPassword = model.ConfirmPassword,

            }, CancellationToken.None);

            if (!response.IsSuccessfull)
            {
                ModelState.AddModelError(string.Empty, response.Message);
                return View(model);
            }

        }

        return RedirectToAction("MailOnay", "Account");
    }


    // Emlakçı kayıt bilgilerini doğrulamak için e-posta gönderir
    [HttpPost]
    public async Task<IActionResult> EstateRegister(EstateRegisterInfoModel model)
    {
        if (!ModelState.IsValid)
        {
            // ModelState hatalarını logla ve yönlendir
            _logger.LogWarning("SendEstateVerifyInfo: ModelState geçersiz. Hatalar: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return View(model); // Hataları View'e geri gönder
        }

        // ✅ KRİTİK GÜVENLİK: Unique email kontrolü
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            ModelState.AddModelError("Email", "Bu e-posta adresi sistemde zaten kayıtlı. Lütfen giriş yapınız veya farklı bir e-posta kullanınız.");
            _logger.LogWarning("EstateRegister: Email already exists - {Email}", model.Email);
            return View(model);
        }

        var response = await _userClientService.CorporateRegister(new CorporateRegisterCommandRequest
        {
            CompanyName = model.CompanyName,
            Email = model.Email,
            PhoneNumber = model.PhoneNumber,
            Password = model.Password,
            Address = model.Address,
            City = model.City,
            District = model.District,
            ConfirmPassword = model.ConfirmPassword,
            FirstName = model.FirstName,
            LastName = model.LastName,
            TaxPlate = model.TaxPlate,
            Level5Certificate = model.Level5Certificate,
            ProfilePicture = model.ProfilePicture,
            Reference = model.Reference,
            UserTypes = model.UserTypes,

        }, CancellationToken.None);


        if (response.IsSuccessfull)
        {
            _logger.LogInformation("Emlakçı doğrulama e-postası gönderildi: {Email}", model.Email);

            return View("SendEstateVerifyInfo"); // Yeni bir onay bekleniyor sayfası
        }

        ModelState.AddModelError(string.Empty, response.Message);
        return View(model);
    }

    // Google ile giriş başlatır
    [HttpGet]
    public IActionResult LoginWithGoogle(string? returnUrl, string type)
    {

        var uri = $"{GeneralConstants.BaseUrl}/Account/GoogleResponse?type={type}";

        AuthenticationProperties properties = _signInManager.ConfigureExternalAuthenticationProperties("Google", uri);

        return new ChallengeResult("Google", properties);
    }


    // Apple ile giriş için yönlendirme
    public IActionResult LoginWithApple(string? returnUrl, string type)
    {
        var uri = $"{GeneralConstants.BaseUrl}/Account/GoogleResponse?type={type}";
        var properties = _signInManager.ConfigureExternalAuthenticationProperties("Apple", uri);
        return new ChallengeResult("Apple", properties);
    }

    // Google giriş yanıtını işler
    public async Task<IActionResult> GoogleResponse(string type, string? returnUrl = "/")
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            _logger.LogWarning("GoogleResponse: Harici giriş bilgisi alınamadı.");
            return RedirectToAction(nameof(Login));
        }

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var fullname = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "";
        var firstname = fullname.Split(' ', 2)[0]; // Sadece ilk kelimeyi al
        string lastname = fullname.Contains(' ') ? fullname.Split(' ', 2)[1] : ""; // Kalan kelimeleri al
        var phone = info.Principal.FindFirstValue(ClaimTypes.MobilePhone) ?? "";
        var socialId = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);


        var response = await _userClientService.SocialRegister(new LoginWithSocialMediaCommandRequest
        {
            Email = email,
            FirstName = firstname,
            LastName = lastname,
            PhoneNumber = phone,
            SocialId = socialId,
            Provider = "Google",
            PhotoUrl = info.Principal.FindFirstValue("picture"), // Google'dan profil resmi URL'si
            Token = info.Principal.FindFirstValue("access_token"), // Google'dan erişim token'ı
            UserType = type

        }, CancellationToken.None);



        if (response.IsSuccessfull)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            
            if (existingUser != null)
            {
                await _signInManager.SignInAsync(existingUser, isPersistent: true);
                _logger.LogInformation("GoogleResponse: Mevcut kullanıcı ({Email}) ile giriş yapıldı.", email);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı Bulunamadı.");

                if (type == "Bireysel")
                {
                    return View("Login");

                }
                else
                {
                    return View("CorporateLogin");

                }
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, response.Message);


            if (type == "Bireysel")
            {
                return View("Login");

            }
            else
            {
                return View("CorporateLogin");

            }
        }


    }
  
    #endregion



    // Kullanıcı kimlik bilgilerini değiştirir (ad, soyad, telefon, şirket adı, profil resmi, şifre)
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangeCredentials(ChangeCredentialsViewModel model)
    {
        if (!ModelState.IsValid)
        {
            // ModelState hatalarını logla ve yönlendir
            _logger.LogWarning("ChangeCredentials: ModelState geçersiz. Hatalar: {Errors}", string.Join(", ", ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage)));
            return RedirectToAction("Settings", new { tab = "profile" });
        }

        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogWarning("ChangeCredentials: Kullanıcı bulunamadı.");
            return RedirectToAction("Login");
        }

        // Kullanıcı bilgilerini güncelle
        user.FirstName = model.FirstName ?? user.FirstName;
        user.LastName = model.LastName ?? user.LastName;
        user.PhoneNumber = model.PhoneNumber ?? user.PhoneNumber;
        user.CompanyName = model.CompanyName ?? user.CompanyName; // Şirket adı güncellemesi

        // Profil resmi işleme
        if (model.ProfilePicture != null)
        {
            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "profiles");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.ProfilePicture.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                await using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    await model.ProfilePicture.CopyToAsync(fileStream);
                }

                // Eski profil resmini sil
                if (!string.IsNullOrEmpty(user.ProfilePicturePath) && user.ProfilePicturePath != "/ImageFiles/boşprofifoto.webp")
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.ProfilePicturePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                user.ProfilePicturePath = "/uploads/profiles/" + uniqueFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Profil resmi güncellenirken hata oluştu.");
                ModelState.AddModelError(string.Empty, "Profil resmi güncellenirken bir hata oluştu.");
            }
        }

        // Banner fotoğrafı işleme
        if (model.BannerPicture != null)
        {
            // Dosyayı hafızaya al; aynı stream'i hem doğrulama hem yazma için kullan
            await using var memory = new MemoryStream();
            await model.BannerPicture.CopyToAsync(memory);
            memory.Position = 0;

            // Sunucu tarafı zorunlu boyut kontrolü: 1220x240
            try
            {
                using var image = System.Drawing.Image.FromStream(memory, useEmbeddedColorManagement: false, validateImageData: false);
                if (image.Width != 1220 || image.Height != 240)
                {
                    ModelState.AddModelError("BannerPicture", "Banner boyutu 1220 x 240 piksel olmalıdır.");
                    return RedirectToAction("Settings", new { tab = "profile" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banner resmi okunamadı veya geçersiz.");
                ModelState.AddModelError("BannerPicture", "Geçersiz resim dosyası.");
                return RedirectToAction("Settings", new { tab = "profile" });
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "banners");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            var uniqueFileName = Guid.NewGuid().ToString() + "_" + model.BannerPicture.FileName;
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            try
            {
                memory.Position = 0;
                await using var fileStream = new FileStream(filePath, FileMode.Create);
                await memory.CopyToAsync(fileStream);

                // Eski banner resmini sil
                if (!string.IsNullOrEmpty(user.BannerPicturePath))
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.BannerPicturePath.TrimStart('/'));
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }

                user.BannerPicturePath = "/uploads/banners/" + uniqueFileName;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Banner resmi güncellenirken hata oluştu.");
                ModelState.AddModelError(string.Empty, "Banner resmi güncellenirken bir hata oluştu.");
            }
        }

        // Şifre değiştirme
        if (!string.IsNullOrEmpty(model.Password))
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var result = await _userManager.ResetPasswordAsync(user, token, model.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                _logger.LogWarning("ChangeCredentials: Şifre değiştirme başarısız. Hatalar: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                return RedirectToAction("Settings", new { tab = "profile" });
            }
        }

        var updateResult = await _userManager.UpdateAsync(user);
        if (!updateResult.Succeeded)
        {
            foreach (var error in updateResult.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
            _logger.LogError("ChangeCredentials: Kullanıcı bilgileri güncellenirken hata oluştu. Hatalar: {Errors}", string.Join(", ", updateResult.Errors.Select(e => e.Description)));
            return RedirectToAction("Settings", new { tab = "profile" });
        }

        return RedirectToAction("Settings", new { tab = "profile" });
    }





    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteBanner()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogWarning("DeleteBanner: Kullanıcı bulunamadı.");
            return RedirectToAction("Login");
        }

        try
        {
            if (!string.IsNullOrEmpty(user.BannerPicturePath))
            {
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", user.BannerPicturePath.TrimStart('/'));
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                user.BannerPicturePath = null;
                await _userManager.UpdateAsync(user);
            }
            TempData["Success"] = "Banner fotoğrafı kaldırıldı.";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "DeleteBanner: Banner silinirken hata oluştu.");
            TempData["Error"] = "Banner silinirken bir hata oluştu.";
        }

        return RedirectToAction("Settings", new { tab = "profile" });
    }

    public IActionResult AccessDenied()
    {
        return View();
    }


    // Apple giriş yanıtını işler
    [HttpGet]
    public async Task<IActionResult> AppleResponse(string type, string returnUrl = null)
    {
        var info = await _signInManager.GetExternalLoginInfoAsync();

        if (info == null)
        {
            _logger.LogWarning("GoogleResponse: Harici giriş bilgisi alınamadı.");
            return RedirectToAction(nameof(Login));
        }

        var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, false);

        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }

        var email = info.Principal.FindFirstValue(ClaimTypes.Email);
        var fullname = info.Principal.FindFirstValue(ClaimTypes.Name) ?? "";
        var firstname = fullname.Split(' ', 2)[0]; // Sadece ilk kelimeyi al
        string lastname = fullname.Contains(' ') ? fullname.Split(' ', 2)[1] : ""; // Kalan kelimeleri al
        var phone = info.Principal.FindFirstValue(ClaimTypes.MobilePhone) ?? "";
        var socialId = info.Principal.FindFirstValue(ClaimTypes.NameIdentifier);


        var response = await _userClientService.SocialRegister(new LoginWithSocialMediaCommandRequest
        {
            Email = email,
            FirstName = firstname,
            LastName = lastname,
            PhoneNumber = phone,
            SocialId = socialId,
            Provider = "Apple",
            PhotoUrl = info.Principal.FindFirstValue("picture"), // Google'dan profil resmi URL'si
            Token = info.Principal.FindFirstValue("access_token"), // Google'dan erişim token'ı
            UserType = type

        }, CancellationToken.None);



        if (response.IsSuccessfull)
        {
            var existingUser = await _userManager.FindByEmailAsync(email);

            if (existingUser != null)
            {
                await _signInManager.SignInAsync(existingUser, isPersistent: true);
                _logger.LogInformation("GoogleResponse: Mevcut kullanıcı ({Email}) ile giriş yapıldı.", email);
                return RedirectToAction("Index", "Home");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Kullanıcı Bulunamadı.");


                if (type == "Bireysel")
                {
                    return View("Login");

                }
                else
                {
                    return View("CorporateLogin");

                }
            }
        }
        else
        {
            ModelState.AddModelError(string.Empty, response.Message);


            if (type == "Bireysel")
            {
                return View("Login");

            }
            else
            {
                return View("CorporateLogin");

            }
        }


    }



    // Yerel bir URL'ye yönlendirir
    private IActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }
        return RedirectToAction("Index", "Home"); // Güvenli fallback
    }


    // Kullanıcı bilgilerini View'e taşır
    public async Task<IActionResult> YourAction()
    {
        var user = await _userManager.GetUserAsync(User); // Asenkron yapıldı

        if (user != null)
        {
            ViewBag.UserFullName = $"{user.FirstName} {user.LastName}";
            ViewBag.PhoneNumber = user.PhoneNumber;
        }
        else
        {
            ViewBag.UserFullName = "Bilinmiyor";
            ViewBag.PhoneNumber = "Numara bulunamadı";
        }

        return View();
    }

    // Kullanıcı ayarları sayfasını gösterir
    [Authorize]
    public async Task<IActionResult> Settings(string tab = "profile")
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            _logger.LogWarning("Ayarlar sayfasına erişim denemesi başarısız, kullanıcı bulunamadı.");
            return RedirectToAction("Login");
        }

        var model = new SettingsViewModel
        {
            Id = user.Id,
            Email = user.Email,
            PhoneNumber = user.PhoneNumber,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CompanyName = user.CompanyName,
            ProfilePicturePath = user.ProfilePicturePath,
            BannerPicturePath = user.BannerPicturePath,
            UserTypes = user.UserTypes,
            IsConsultant = user.IsConsultant,
            Consultants = user.CompanyName != null ? await GetConsultantsForCompany(user.CompanyName) : new List<ApplicationUser>(), // Null kontrolü eklendi
            City = user.City,
            District = user.District,
            AcikAdres = user.AcikAdres
        };

        ViewData["ActiveTab"] = tab;
        return View(model);
    }




    #region Consultant




    // Bir şirkete ait danışmanları alır
    private async Task<List<ApplicationUser>> GetConsultantsForCompany(string companyName)
    {
        if (string.IsNullOrEmpty(companyName)) return new List<ApplicationUser>();

        // Sadece gerekli alanları seçerek performansı artır
        var consultants = await _userManager.Users
            .AsNoTracking() // Takip edilmeyen sorgu
            .Where(u => u.IsConsultant && u.CompanyName == companyName)
            .Select(u => new ApplicationUser // Yalnızca gerekli alanları seç
            {
                Id = u.Id,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                FirstName = u.FirstName,
                LastName = u.LastName,
                EmailConfirmed = u.EmailConfirmed,
                CompanyName = u.CompanyName // CompanyName'i de çekmek gerekebilir
            })
            .ToListAsync();

        return consultants;
    }

    // Bir danışmanı siler
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConsultant(string consultantId)
    {
        var company = await _userManager.GetUserAsync(User);
        if (company == null || company.UserTypes != "KURUMSAL GIRIŞ" || company.IsConsultant)
        {
            TempData["Error"] = "Yetkisiz erişim.";
            _logger.LogWarning("DeleteConsultant: Yetkisiz erişim denemesi. Kullanıcı: {UserName}", User.Identity?.Name);
            return RedirectToAction("Settings");
        }

        var consultant = await _userManager.FindByIdAsync(consultantId);
        if (consultant == null || !consultant.IsConsultant)
        {
            TempData["Error"] = "Danışman bulunamadı.";
            _logger.LogWarning("DeleteConsultant: Silinmek istenen danışman bulunamadı veya danışman değil. ID: {ConsultantId}", consultantId);
            return RedirectToAction("Settings");
        }

        var companyNormalizedName = NormalizeCompanyName(company.CompanyName);
        var consultantNormalizedName = NormalizeCompanyName(consultant.CompanyName);

        if (companyNormalizedName != consultantNormalizedName)
        {
            TempData["Error"] = "Bu danışmanı silme yetkiniz yok.";
            _logger.LogWarning("DeleteConsultant: Danışman ({ConsultantId}) silme yetkisi yok. Şirket adı uyuşmuyor.", consultantId);
            return RedirectToAction("Settings");
        }

        var result = await _userManager.DeleteAsync(consultant);
        if (result.Succeeded)
        {
            TempData["Success"] = "Danışman başarıyla silindi.";
            _logger.LogInformation("Danışman ({ConsultantId}) başarıyla silindi.", consultantId);
        }
        else
        {
            TempData["Error"] = "Danışman silinirken bir hata oluştu.";
            _logger.LogError("Danışman ({ConsultantId}) silinirken hata oluştu: {Errors}", consultantId, string.Join(", ", result.Errors.Select(e => e.Description)));
        }

        return RedirectToAction("Settings");
    }

    // Danışman daveti gönderir
    [HttpPost]
    public async Task<IActionResult> InviteConsultant([FromBody] InviteConsultantViewModel model)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                _logger.LogWarning("InviteConsultant: ModelState geçersiz. Hatalar: {Errors}", string.Join(", ", errors));
                return Json(new { success = false, message = "Geçersiz form verisi." });
            }

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null || currentUser.UserTypes != "KURUMSAL GIRIŞ" || currentUser.IsConsultant)
            {
                _logger.LogWarning("InviteConsultant: Yetkisiz erişim denemesi. Kullanıcı: {UserName}", User.Identity?.Name);
                return Json(new { success = false, message = "Yetkisiz erişim." });
            }

            var existingUser = await _userManager.FindByEmailAsync(model.Email);
            if (existingUser != null)
            {
                return Json(new { success = false, message = "Bu e-posta adresi zaten kullanımda." });
            }

            var invitationToken = await GenerateInvitationToken(model.Email);

            // ✅ Kurumsal kullanıcılar için tip bazlı sıra numarası al
            // ŞEVVAL EMLAK K-0001, diğerleri sırayla
            var maxKurumsalOrder = await _userManager.Users
                .AsNoTracking()
                .Where(u => u.UserTypes != "Bireysel")
                .OrderByDescending(u => u.UserOrder)
                .FirstOrDefaultAsync();
            int newUserOrder = (maxKurumsalOrder?.UserOrder ?? 0) + 1;

            var consultant = new ConsultantInvitation
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                InvitationToken = invitationToken,
                InvitedBy = currentUser.Id,
                CompanyName = currentUser.CompanyName,
                CreatedAt = DateTime.UtcNow,
                ExpiryDate = DateTime.UtcNow.AddDays(7),
                Status = "Pending",
                UserOrder = newUserOrder,
                ProfilePicturePath = string.IsNullOrEmpty(model.ProfilePicturePath) ? "/ImageFiles/boşprofifoto.webp" : model.ProfilePicturePath
            };

            _context.ConsultantInvitations.Add(consultant);
            await _context.SaveChangesAsync();

            var setPasswordUrl = Url.Action("SetConsultantPassword", "Account",
                new { token = invitationToken, email = model.Email }, Request.Scheme);

            await _emailService.SendConsultantInvitationEmailAsync(new ConsultantInvitationEmailDto
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                SetPasswordUrl = setPasswordUrl,
                CompanyName = currentUser.CompanyName
            });

            _logger.LogInformation("Danışman daveti başarıyla gönderildi: {Email}", model.Email);
            return Json(new { success = true, message = "Danışman daveti başarıyla gönderildi." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "InviteConsultant sırasında beklenmeyen bir hata oluştu.");
            return Json(new { success = false, message = "Beklenmeyen bir hata oluştu. Lütfen tekrar deneyiniz." });
        }
    }

    // Davet token'ı üretir
    private async Task<string> GenerateInvitationToken(string email)
    {
        using var rng = RandomNumberGenerator.Create(); // Yeni ve daha güvenli
        var tokenBytes = new byte[32];
        string token;
        do
        {
            rng.GetBytes(tokenBytes);
            token = Convert.ToBase64String(tokenBytes)
                .Replace("+", "-")
                .Replace("/", "_")
                .Replace("=", "");
        } while (await _context.ConsultantInvitations.AsNoTracking().AnyAsync(i => i.InvitationToken == token)); // AsNoTracking ile performans

        return token;
    }

    // Danışman şifre belirleme sayfasını gösterir
    [HttpGet]
    public async Task<IActionResult> SetConsultantPassword(string token)
    {
        if (string.IsNullOrEmpty(token))
        {
            return RedirectToAction("Login");
        }

        var invitation = await _context.ConsultantInvitations
            .AsNoTracking() // Takip edilmeyen sorgu
            .FirstOrDefaultAsync(i => i.InvitationToken == token && i.Status == "Pending" && i.ExpiryDate > DateTime.UtcNow);

        if (invitation == null)
        {
            TempData["Error"] = "Geçersiz veya süresi dolmuş davet linki.";
            _logger.LogWarning("SetConsultantPassword: Geçersiz veya süresi dolmuş davet linki kullanıldı.");
            return RedirectToAction("Login");
        }

        ViewBag.Token = token;
        return View();
    }

    // Danışman şifresini belirler ve hesabı aktifleştirir
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SetConsultantPassword(string token, string password, string confirmPassword)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(password) || password != confirmPassword)
        {
            TempData["Error"] = "Geçersiz şifre veya şifreler eşleşmiyor.";
            ViewBag.Token = token;
            return View();
        }

        var invitation = await _context.ConsultantInvitations
            .FirstOrDefaultAsync(i => i.InvitationToken == token && i.Status == "Pending" && i.ExpiryDate > DateTime.UtcNow);

        if (invitation == null)
        {
            TempData["Error"] = "Geçersiz veya süresi dolmuş davet linki.";
            _logger.LogWarning("SetConsultantPassword: Geçersiz veya süresi dolmuş davet linki kullanıldı (POST).");
            return RedirectToAction("Login");
        }

        try
        {
            var user = await _userManager.FindByEmailAsync(invitation.Email);
            if (user == null)
            {
                // ✅ Kurumsal danışman için tip bazlı sıra numarası al
                // ŞEVVAL EMLAK K-0001, diğer kurumsallar sırayla
                var maxKurumsalOrder = await _userManager.Users
                    .AsNoTracking()
                    .Where(u => u.UserTypes != "Bireysel")
                    .OrderByDescending(u => u.UserOrder)
                    .FirstOrDefaultAsync();
                int newUserOrder = (maxKurumsalOrder?.UserOrder ?? 0) + 1;

                user = new ApplicationUser
                {
                    UserName = invitation.Email,
                    Email = invitation.Email,
                    FirstName = invitation.FirstName,
                    LastName = invitation.LastName,
                    UserTypes = "KURUMSAL GIRIŞ",
                    IsSubscribed = "ücretsiz",
                    EmailConfirmed = true,
                    IsConsultant = true,
                    RegistrationDate = DateTime.UtcNow,
                    IsActive = "active",
                    UserOrder = newUserOrder,
                    ProfilePicturePath = "/ImageFiles/boşprofifoto.webp",
                    CompanyName = invitation.CompanyName // Şirket adını davetiyeden al
                };

                var result = await _userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                {
                    TempData["Error"] = "Hesap oluşturulurken bir hata oluştu.";
                    _logger.LogError("SetConsultantPassword: Yeni danışman hesabı oluşturulurken hata oluştu: {Errors}", string.Join(", ", result.Errors.Select(e => e.Description)));
                    ViewBag.Token = token;
                    return View();
                }
            }
            else
            {
                // Mevcut kullanıcıyı danışman olarak güncelle
                user.UserTypes = "KURUMSAL GIRIŞ";
                user.IsConsultant = true;
                user.ProfilePicturePath = "/ImageFiles/boşprofifoto.webp";
                user.CompanyName = invitation.CompanyName; // Şirket adını davetiyeden al
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    TempData["Error"] = "Kullanıcı bilgileri güncellenirken hata oluştu.";
                    _logger.LogError("SetConsultantPassword: Mevcut danışman güncellenirken hata oluştu: {Errors}", string.Join(", ", updateResult.Errors.Select(e => e.Description)));
                    ViewBag.Token = token;
                    return View();
                }

                var resetToken = await _userManager.GeneratePasswordResetTokenAsync(user);
                var passwordResult = await _userManager.ResetPasswordAsync(user, resetToken, password);
                if (!passwordResult.Succeeded)
                {
                    TempData["Error"] = "Şifre güncellenirken bir hata oluştu.";
                    _logger.LogError("SetConsultantPassword: Mevcut danışman şifresi güncellenirken hata oluştu: {Errors}", string.Join(", ", passwordResult.Errors.Select(e => e.Description)));
                    ViewBag.Token = token;
                    return View();
                }
            }

            invitation.Status = "Completed";
            await _context.SaveChangesAsync();

            await _signInManager.SignInAsync(user, isPersistent: true);

            TempData["Success"] = "Hesabınız başarıyla aktifleştirildi.";
            _logger.LogInformation("Danışman hesabı ({Email}) başarıyla aktifleştirildi.", user.Email);
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Hesap aktivasyonu sırasında bir hata oluştu: " + ex.Message;
            _logger.LogError(ex, "Hesap aktivasyonu sırasında beklenmeyen bir hata oluştu.");
            ViewBag.Token = token;
            return View();
        }
    }

    // Danışmanları listeler
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> ListConsultants()
    {
        var company = await _userManager.GetUserAsync(User);
        if (company == null || company.UserTypes != "KURUMSAL GIRIŞ" || company.IsConsultant)
        {
            _logger.LogWarning("ListConsultants: Yetkisiz erişim denemesi. Kullanıcı: {UserName}", User.Identity?.Name);
            return Json(new { success = false, message = "Yetkisiz erişim." });
        }

        // Sadece gerekli alanları seçerek performansı artır
        var consultants = await _userManager.Users
            .AsNoTracking() // Takip edilmeyen sorgu
            .Where(u => u.IsConsultant && u.CompanyName == company.CompanyName)
            .Select(u => new ConsultantViewModel
            {
                Id = u.Id,
                Email = u.Email,
                PhoneNumber = u.PhoneNumber,
                FirstName = u.FirstName,
                LastName = u.LastName,
                IsActive = u.EmailConfirmed, // EmailConfirmed, hesabın aktifliği olarak kullanılabilir
                CompanyName = u.CompanyName
            })
            .ToListAsync();

        return Json(new { success = true, consultants = consultants });
    }

    #endregion


    #region Membership





    // Üyelik değişim talebi oluşturur
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RequestMembershipChange(
    string requestedType,
    string? companyName, // Nullable yapıldı
    string? companyAddress, // Nullable yapıldı
    string? taxNumber, // Nullable yapıldı
    string? phoneNumber, // Nullable yapıldı
    IFormFile? level5Certificate, // Nullable yapıldı
    IFormFile? taxDocument) // Nullable yapıldı
    {
        _logger.LogInformation("RequestMembershipChange: Kullanıcı {UserName} için methoda giriliyor.", User.Identity?.Name);

        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            _logger.LogWarning("ModelState geçersiz. Hatalar: {Errors}", string.Join(", ", errors));
            return BadRequest(new
            {
                success = false,
                message = "Invalid input.",
                errors
            });
        }

        try
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                _logger.LogWarning("RequestMembershipChange: Kullanıcı bulunamadı.");
                return Unauthorized(new
                {
                    success = false,
                    message = "User not found."
                });
            }

            if (string.IsNullOrWhiteSpace(requestedType))
            {
                _logger.LogWarning("RequestMembershipChange: Geçersiz üyelik tipi.");
                return BadRequest(new
                {
                    success = false,
                    message = "Invalid membership type."
                });
            }

            var existingRequest = await _context.MembershipChangeRequests
                .AsNoTracking() // Takip edilmeyen sorgu
                .FirstOrDefaultAsync(r => r.UserId == user.Id && r.Status == "Pending");

            if (existingRequest != null)
            {
                _logger.LogWarning("RequestMembershipChange: Kullanıcı {UserId} için bekleyen bir talep bulundu.", user.Id);
                return BadRequest(new
                {
                    success = false,
                    message = "A pending membership change request already exists."
                });
            }

            string? level5CertificatePath = null;
            string? taxDocumentPath = null;

            if (requestedType == "KURUMSAL GIRIŞ")
            {
                if (level5Certificate == null || taxDocument == null)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Level 5 Certificate and Tax Document are mandatory for Corporate membership."
                    });
                }

                var allowedExtensions = new[] { ".pdf", ".webp", ".webp", ".webp" };
                var level5Ext = Path.GetExtension(level5Certificate.FileName).ToLowerInvariant();
                var taxExt = Path.GetExtension(taxDocument.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(level5Ext) || !allowedExtensions.Contains(taxExt))
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Only PDF, JPG, JPEG, and PNG files are accepted for documents."
                    });
                }

                const long maxFileSize = 5 * 1024 * 1024; // 5MB
                if (level5Certificate.Length > maxFileSize || taxDocument.Length > maxFileSize)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "File size cannot exceed 5MB."
                    });
                }

                if (string.IsNullOrWhiteSpace(companyName) ||
                    string.IsNullOrWhiteSpace(companyAddress) ||
                    string.IsNullOrWhiteSpace(taxNumber) ||
                    string.IsNullOrWhiteSpace(phoneNumber))
                {
                    _logger.LogWarning("RequestMembershipChange: Kurumsal üyelik için gerekli alanlar eksik.");
                    return BadRequest(new
                    {
                        success = false,
                        message = "Please fill in all fields for corporate membership."
                    });
                }

                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "membership_docs");
                Directory.CreateDirectory(uploadsFolder); // Klasör yoksa oluştur

                var level5FileName = $"level5_{user.Id}_{DateTime.UtcNow.Ticks}{Path.GetExtension(level5Certificate.FileName)}";
                level5CertificatePath = Path.Combine("/uploads/membership_docs/", level5FileName);
                var taxFileName = $"tax_{user.Id}_{DateTime.UtcNow.Ticks}{Path.GetExtension(taxDocument.FileName)}";
                taxDocumentPath = Path.Combine("/uploads/membership_docs/", taxFileName);

                try
                {
                    await using (var level5Stream = new FileStream(Path.Combine(uploadsFolder, level5FileName), FileMode.Create))
                    {
                        await level5Certificate.CopyToAsync(level5Stream);
                    }

                    await using (var taxStream = new FileStream(Path.Combine(uploadsFolder, taxFileName), FileMode.Create))
                    {
                        await taxDocument.CopyToAsync(taxStream);
                    }
                }
                catch (Exception fileEx)
                {
                    _logger.LogError(fileEx, "Üyelik belgeleri kaydedilirken hata oluştu.");
                    return StatusCode(500, new
                    {
                        success = false,
                        message = "An error occurred while saving documents."
                    });
                }
            }

            var request = new MembershipChangeRequest
            {
                UserId = user.Id,
                CurrentType = user.UserTypes,
                RequestedType = requestedType,
                Status = "Pending",
                CompanyName = companyName ?? string.Empty,
                CompanyAddress = companyAddress ?? string.Empty,
                TaxNumber = taxNumber ?? string.Empty,
                PhoneNumber = phoneNumber ?? string.Empty,
                RequestDate = DateTime.UtcNow,
                Level5CertificatePath = level5CertificatePath,
                TaxDocumentPath = taxDocumentPath
            };

            _context.MembershipChangeRequests.Add(request);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Üyelik değişim talebi oluşturuldu: Kullanıcı {UserId}, Tip {RequestedType}", user.Id, requestedType);

            // Admin ve kullanıcıya e-posta gönder
            var approveLink = Url.Action("ApproveMembershipRequest", "Account",
                new { id = request.Id, token = GenerateRequestToken(request.Id, "approve") },
                Request.Scheme);
            var rejectLink = Url.Action("RejectMembershipRequest", "Account",
                new { id = request.Id, token = GenerateRequestToken(request.Id, "reject") },
                Request.Scheme);

            var emailBody = $@"
                <div style='font-family: Arial, sans-serif; max-width: 600px; margin: 0 auto; padding: 20px;'>
                    <h2 style='color: #333; border-bottom: 2px solid #eee; padding-bottom: 10px;'>Yeni Üyelik Değişim Talebi</h2>
    
                    <div style='background-color: #f8f9fa; padding: 20px; border-radius: 5px; margin: 20px 0;'>
                        <p><strong>Kullanıcı:</strong> {user.Email}</p>
                        <p><strong>Mevcut Üyelik:</strong> {request.CurrentType}</p>
                        <p><strong>İstenen Üyelik:</strong> {request.RequestedType}</p>";

            if (requestedType == "KURUMSAL GIRIŞ")
            {
                emailBody += $@"
                <p><strong>Şirket Adı:</strong> {request.CompanyName}</p>
                <p><strong>Vergi No:</strong> {request.TaxNumber}</p>
                <p><strong>Telefon:</strong> {request.PhoneNumber}</p>
                <div style='margin-top: 15px;'>
                    <p><strong>Belgeler:</strong></p>
                    <p><a href='{Request.Scheme}://{Request.Host}{request.Level5CertificatePath}' style='color: #007bff; text-decoration: none;'>
                        <span style='background: #e9ecef; padding: 5px 10px; border-radius: 3px;'>📄 Seviye 5 Belgesi</span>
                    </a></p>
                    <p><a href='{Request.Scheme}://{Request.Host}{request.TaxDocumentPath}' style='color: #007bff; text-decoration: none;'>
                        <span style='background: #e9ecef; padding: 5px 10px; border-radius: 3px;'>📄 Vergi Levhası</span>
                    </a></p>
                </div>";
            }

            emailBody += $@"
            </div>

            <div style='text-align: center; margin: 30px 0;'>
                <a href='{approveLink}' style='display: inline-block; padding: 12px 25px; margin: 0 10px; background-color: #28a745; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                    ✓ Talebi Onayla
                </a>
                <a href='{rejectLink}' style='display: inline-block; padding: 12px 25px; margin: 0 10px; background-color: #dc3545; color: white; text-decoration: none; border-radius: 5px; font-weight: bold;'>
                    ✗ Talebi Reddet
                </a>
            </div>

            <div style='margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee;'>
                <p style='color: #666; font-size: 14px; margin: 0;'>
                    Bu e-posta otomatik olarak gönderilmiştir. Lütfen yanıtlamayınız.
                </p>
            </div>
        </div>";

            var adminEmail = _configuration["AdminEmail"] ?? "sevvaldestek@gmail.com"; // Admin e-postası config'den alınabilir
            await _emailService.SendEmailAsync(adminEmail,
                "Yeni Üyelik Değişim Talebi",
                emailBody);
            await _emailService.SendEmailAsync(user.Email,
                "Üyelik Değişim Talebiniz Alındı",
                "Üyelik değişim talebiniz başarıyla alındı. Talebiniz incelendikten sonra size bilgi verilecektir.");

            return Json(new
            {
                success = true,
                message = "Your membership change request has been successfully submitted. You will be notified via email."
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "RequestMembershipChange sırasında hata oluştu: Kullanıcı {UserName}", User.Identity?.Name);
            return StatusCode(500, new
            {
                success = false,
                message = "An error occurred: " + ex.Message
            });
        }
    }

    // Bekleyen üyelik değişim talebini iptal eder
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CancelMembershipRequest()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null)
        {
            return Json(new { success = false, message = "Kullanıcı bulunamadı." });
        }

        var request = await _context.MembershipChangeRequests
            .FirstOrDefaultAsync(r => r.UserId == user.Id && r.Status == "Pending");

        if (request == null)
        {
            return Json(new { success = false, message = "Aktif bir üyelik değişim talebi bulunamadı." });
        }

        request.Status = "Cancelled";
        request.ProcessDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        _logger.LogInformation("Üyelik değişim talebi iptal edildi: Kullanıcı {UserId}", user.Id);

        return Json(new { success = true, message = "Üyelik değişim talebiniz iptal edildi." });
    }

    // Üyelik değişim talebini işler (Admin rolü gerektirir)
    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessMembershipRequest(int id, string action, string note)
    {
        var request = await _context.MembershipChangeRequests
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null)
        {
            return Json(new { success = false, message = "Talep bulunamadı." });
        }

        if (request.Status != "Pending")
        {
            return Json(new { success = false, message = "Bu talep zaten işlenmiş." });
        }

        request.Status = action; // "Approved" veya "Rejected"
        request.ProcessDate = DateTime.UtcNow;
        request.AdminNote = note;

        if (action == "Approved")
        {
            var user = request.User; // Include sayesinde user zaten yüklü
            if (user != null)
            {
                user.UserTypes = request.RequestedType;
                await _userManager.UpdateAsync(user);
                _logger.LogInformation("Üyelik değişim talebi onaylandı: Kullanıcı {UserId}, Yeni Tip: {NewType}", user.Id, request.RequestedType);
            }

            await _emailService.SendEmailAsync(request.User.Email,
                "Üyelik Değişim Talebiniz Onaylandı",
                $"Üyelik değişim talebiniz onaylandı. Yeni üyelik tipiniz: {request.RequestedType}" +
                (!string.IsNullOrEmpty(note) ? $"<br><br>Admin Notu: {note}" : ""));
        }
        else if (action == "Rejected")
        {
            _logger.LogInformation("Üyelik değişim talebi reddedildi: Kullanıcı {UserId}", request.User.Id);
            await _emailService.SendEmailAsync(request.User.Email,
                "Üyelik Değişim Talebiniz Reddedildi",
                $"Üyelik değişim talebiniz reddedildi." +
                (!string.IsNullOrEmpty(note) ? $"<br><br>Admin Notu: {note}" : ""));
        }

        await _context.SaveChangesAsync();

        return Json(new { success = true, message = $"Talep başarıyla {action.ToLower()} edildi." });
    }

    // Bekleyen üyelik değişim taleplerini listeler (Admin rolü gerektirir)
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> MembershipRequests()
    {
        var requests = await _context.MembershipChangeRequests
            .Include(r => r.User) // Kullanıcı bilgilerini de yükle
            .AsNoTracking() // Takip edilmeyen sorgu
            .OrderByDescending(r => r.RequestDate)
            .ToListAsync();

        return View(requests);
    }

    // Onay/Reddetme linkleri için token üretir
    private string GenerateRequestToken(int requestId, string action)
    {
        // Token'a timestamp ekleyerek benzersizlik ve süresi dolmuşluk kontrolü sağlar
        var payload = $"{requestId}:{action}:{DateTime.UtcNow.Ticks}";
        return Convert.ToBase64String(Encoding.UTF8.GetBytes(payload));
    }

    // Token'ı doğrular ve ID ile action'ı döner
    private (int requestId, string action, bool isValid) ValidateRequestToken(string token)
    {
        try
        {
            var decoded = Encoding.UTF8.GetString(Convert.FromBase64String(token));
            var parts = decoded.Split(':');
            if (parts.Length == 3 && int.TryParse(parts[0], out int requestId))
            {
                var timestamp = long.Parse(parts[2]);
                var tokenGenerationTime = new DateTime(timestamp, DateTimeKind.Utc);
                // Token'ın 24 saatten eski olup olmadığını kontrol et
                if (DateTime.UtcNow - tokenGenerationTime <= TimeSpan.FromHours(24))
                {
                    return (requestId, parts[1], true);
                }
            }
        }
        catch (FormatException)
        {
            _logger.LogWarning("Geçersiz Base64 token formatı: {Token}", token);
        }
        catch (IndexOutOfRangeException)
        {
            _logger.LogWarning("Token parçalara ayrılamadı veya eksik: {Token}", token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Token doğrulanırken beklenmeyen bir hata oluştu: {Token}", token);
        }
        return (-1, string.Empty, false); // Geçersizse -1 ve boş action döner
    }


    // Üyelik değişim talebini onaylar (doğrudan link ile)
    [HttpGet]
    public async Task<IActionResult> ApproveMembershipRequest(int id, string token)
    {
        var (validRequestId, action, isValid) = ValidateRequestToken(token);
        if (!isValid || validRequestId != id || action != "approve")
        {
            return BadRequest("Geçersiz veya süresi dolmuş bağlantı.");
        }

        var request = await _context.MembershipChangeRequests
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null || request.Status != "Pending")
        {
            return NotFound("Talep bulunamadı veya zaten işlenmiş.");
        }

        request.Status = "Approved";
        request.ProcessDate = DateTime.UtcNow;

        var user = request.User;
        if (user != null)
        {
            user.UserTypes = request.RequestedType;
            await _userManager.UpdateAsync(user);
        }

        await _context.SaveChangesAsync();

        await _emailService.SendEmailAsync(user?.Email ?? request.User.Email, // Null kontrolü
            "Üyelik Değişim Talebiniz Onaylandı",
            $"Üyelik değişim talebiniz onaylandı. Yeni üyelik tipiniz: {request.RequestedType}");

        TempData["Message"] = "Üyelik değişim talebi başarıyla onaylandı.";
        return RedirectToAction("MembershipRequests"); // Admin sayfasına yönlendir
    }

    // Üyelik değişim talebini reddeder (doğrudan link ile)
    [HttpGet]
    public async Task<IActionResult> RejectMembershipRequest(int id, string token)
    {
        var (validRequestId, action, isValid) = ValidateRequestToken(token);
        if (!isValid || validRequestId != id || action != "reject")
        {
            return BadRequest("Geçersiz veya süresi dolmuş bağlantı.");
        }

        var request = await _context.MembershipChangeRequests
            .Include(r => r.User)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (request == null || request.Status != "Pending")
        {
            return NotFound("Talep bulunamadı veya zaten işlenmiş.");
        }

        request.Status = "Rejected";
        request.ProcessDate = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        await _emailService.SendEmailAsync(request.User?.Email ?? request.User.Email, // Null kontrolü
            "Üyelik Değişim Talebiniz Reddedildi",
            "Üyelik değişim talebiniz reddedildi. Daha fazla bilgi için lütfen bizimle iletişime geçin.");

        TempData["Message"] = "Üyelik değişim talebi başarıyla reddedildi.";
        return RedirectToAction("MembershipRequests"); // Admin sayfasına yönlendir
    }

    #endregion









    #region Bitenler

    private string NormalizeUsername(string input)
    {
        // Türkçe karakterleri İngilizce eşdeğerlerine dönüştürür
        return input.Replace("ç", "c")
                .Replace("ğ", "g")
                .Replace("ı", "i")
                .Replace("ö", "o")
                .Replace("ş", "s")
                .Replace("ü", "u")
                .Replace("Ç", "C")
                .Replace("Ğ", "G")
                .Replace("İ", "I")
                .Replace("Ö", "O")
                .Replace("Ş", "S")
                .Replace("Ü", "U");
    }

    private string NormalizeCompanyName(string companyName)
    {
        if (string.IsNullOrEmpty(companyName)) return string.Empty;

        var normalized = companyName.Replace(" ", "").ToLowerInvariant();

        normalized = normalized
            .Replace("ı", "i")
            .Replace("ğ", "g")
            .Replace("ü", "u")
            .Replace("ş", "s")
            .Replace("ö", "o")
            .Replace("ç", "c")
            .Replace("İ", "i")
            .Replace("Ğ", "g")
            .Replace("Ü", "u")
            .Replace("Ş", "s")
            .Replace("Ö", "o")
            .Replace("Ç", "c");

        return normalized;
    }

    #endregion

    #region Hesap Silme

    /// <summary>
    /// Kullanıcı hesap silme action - POST
    /// </summary>
    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteAccount(DeleteAccountViewModel model)
    {
        try
        {
            // Model validasyonu
            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Lütfen tüm alanları doğru şekilde doldurunuz.";
                return RedirectToAction("Settings", new { tab = "security" });
            }

            // Onay metni kontrolü (case-sensitive)
            if (model.ConfirmationText != "HESABIMI SIL")
            {
                TempData["ErrorMessage"] = "Onay metni yanlış. Tam olarak 'HESABIMI SIL' yazmalısınız (büyük harfle).";
                return RedirectToAction("Settings", new { tab = "security" });
            }

            // Mevcut kullanıcıyı al
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                TempData["ErrorMessage"] = "Kullanıcı bulunamadı.";
                return RedirectToAction("Login");
            }

            // Şifre doğrulama
            var passwordValid = await _userManager.CheckPasswordAsync(user, model.Password);
            if (!passwordValid)
            {
                TempData["ErrorMessage"] = "Şifreniz yanlış. Lütfen tekrar deneyin.";
                _logger.LogWarning("Hesap silme başarısız: Yanlış şifre. UserId: {UserId}", userId);
                return RedirectToAction("Settings", new { tab = "security" });
            }

            // API üzerinden hesap silme işlemini tetikle
            try
            {
                var deleteResult = await _userClientService.DeleteUser(userId, model.Password, model.ConfirmationText);

                if (deleteResult != null && deleteResult.IsSuccessfull)
                {
                    // Kullanıcıyı logout et
                    await _signInManager.SignOutAsync();
                    HttpContext.Session.Clear();

                    _logger.LogInformation("Hesap başarıyla silindi. UserId: {UserId}, Email: {Email}", 
                        userId, user.Email);

                    TempData["SuccessMessage"] = "Hesabınız başarıyla silindi. 30 gün içinde destek@sevval.com adresine e-posta göndererek hesabınızı kurtarabilirsiniz.";
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    TempData["ErrorMessage"] = deleteResult?.Message ?? "Hesap silme işlemi başarısız oldu. Lütfen daha sonra tekrar deneyin.";
                    _logger.LogError("Hesap silme API hatası. UserId: {UserId}, Message: {Message}", 
                        userId, deleteResult?.Message);
                    return RedirectToAction("Settings", new { tab = "security" });
                }
            }
            catch (Exception apiEx)
            {
                _logger.LogError(apiEx, "Hesap silme API çağrısı sırasında hata. UserId: {UserId}", userId);
                TempData["ErrorMessage"] = "Bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
                return RedirectToAction("Settings", new { tab = "security" });
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hesap silme işlemi sırasında beklenmeyen hata");
            TempData["ErrorMessage"] = "Beklenmeyen bir hata oluştu. Lütfen daha sonra tekrar deneyin.";
            return RedirectToAction("Settings", new { tab = "security" });
        }
    }

    /// <summary>
    /// Hesap silme onay modal için veri döndürür (AJAX)
    /// </summary>
    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetDeleteAccountConfirmInfo()
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return Json(new { success = false, message = "Kullanıcı bulunamadı" });
            }

            // Kullanıcının ilan sayısını al (sadece aktif ilanlar)
            var adsCount = await _context.IlanBilgileri
                .Where(x => x.Email == user.Email && x.Status != "deleted")
                .CountAsync();

            // Kullanıcının okunmamış mesaj sayısını al (Message entity'de ReceiverEmail var)
            var unreadMessagesCount = await _context.Messages
                .Where(x => x.ReceiverEmail == user.Email && !x.IsRead)
                .CountAsync();

            var confirmModel = new DeleteAccountConfirmViewModel
            {
                Email = user.Email,
                FullName = $"{user.FirstName} {user.LastName}",
                TotalAdsCount = adsCount,
                UnreadMessagesCount = unreadMessagesCount,
                RecoveryPeriodDays = 30
            };

            return Json(new { success = true, data = confirmModel });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hesap silme onay bilgileri alınırken hata");
            return Json(new { success = false, message = "Bir hata oluştu" });
        }
    }

    #endregion

    #region Hesap Kurtarma

    /// <summary>
    /// Email linkinden hesap kurtarma - GET
    /// Token bazlı hesap aktivasyonu
    /// </summary>
    [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> RecoverAccount(string token, string userId)
    {
        try
        {
            // Parametreleri validate et
            if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(userId))
            {
                TempData["ErrorMessage"] = "Geçersiz kurtarma linki. Lütfen email'inizdeki linki kontrol edin.";
                return RedirectToAction("Login");
            }

            // DeletedAccounts kaydını bul
            var deletedAccount = await _context.DeletedAccounts
                .Include(d => d.User)
                .FirstOrDefaultAsync(d => d.UserId == userId && d.RecoveryToken == token);

            if (deletedAccount == null)
            {
                TempData["ErrorMessage"] = "Kurtarma linki geçersiz veya süresi dolmuş. Lütfen giriş yaparak hesabınızı kurtarmayı deneyin.";
                return RedirectToAction("Login");
            }

            // 30 gün kontrolü
            var daysSinceDeletion = (DateTime.UtcNow - deletedAccount.DeletedAt).TotalDays;
            if (daysSinceDeletion > 30)
            {
                TempData["ErrorMessage"] = "Hesabınız 30 günlük kurtarma süresini aştığı için kalıcı olarak silinmiştir.";
                return RedirectToAction("Login");
            }

            var user = deletedAccount.User;
            
            // RESTORE ACCOUNT
            user.IsActive = "active";
            await _userManager.UpdateAsync(user);

            // Restore user's IlanBilgileri
            var userAds = await _context.IlanBilgileri
                .Where(i => i.Email == user.Email && i.Status == "deleted")
                .ToListAsync();

            foreach (var ad in userAds)
            {
                ad.Status = "active";
            }

            // Remove from DeletedAccounts table
            _context.DeletedAccounts.Remove(deletedAccount);
            await _context.SaveChangesAsync();

            // Auto sign in user
            await _signInManager.SignInAsync(user, isPersistent: true);

            TempData["SuccessMessage"] = $"🎉 Harika! Hesabınız başarıyla kurtarıldı. Tekrar aramızda olduğunuz için mutluyuz, {user.FirstName}!";
            
            _logger.LogInformation($"Account recovered via email link: {user.Email} (ID: {user.Id})");
            
            return RedirectToAction("Index", "Home");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Hesap kurtarma sırasında hata");
            TempData["ErrorMessage"] = "Hesap kurtarma sırasında bir hata oluştu. Lütfen giriş yaparak tekrar deneyin.";
            return RedirectToAction("Login");
        }
    }

    #endregion


}
