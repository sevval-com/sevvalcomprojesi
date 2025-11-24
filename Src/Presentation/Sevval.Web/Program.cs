using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Server.Kestrel.Core;
using AspNet.Security.OAuth.Apple;
using Microsoft.EntityFrameworkCore;
using Sevval.Application.Interfaces.IService;
using Sevval.Domain.Entities;
using Sevval.Infrastructure.Services;
using Sevval.Persistence.Context;
using sevvalemlak.csproj;
using System.Globalization;
using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using Sevval.Application.Constants;

var builder = WebApplication.CreateBuilder(args);


builder.Services.Configure<IISServerOptions>(options =>
{
    options.MaxRequestBodySize = 1073741824; // Set to 1 GB
});

builder.Services.Configure<KestrelServerOptions>(options =>
{
    options.Limits.MaxRequestBodySize = 1073741824; // Set to 1 GB
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 1073741824; // Set to 1 GB
});



// Yerelleþtirme ayarlarý
var supportedCultures = new List<CultureInfo>
{
    new CultureInfo("tr-TR") // Türkçe'yi destekliyoruz
};

// Servisleri ekle

builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
builder.Services.AddRazorPages();
builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
builder.Services.AddHttpClient(); // Add HttpClientFactory for social login
builder.Services.AddScoped<SmtpClient>(serviceProvider =>

{
    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
    return new SmtpClient(configuration["Email:SmtpServer"])
    {
        Port = int.Parse(configuration["Email:SmtpPort"]),
        Credentials = new NetworkCredential(
            configuration["Email:Username"],
            configuration["Email:Password"]
        ),
        EnableSsl = true
    };
});
builder.Services.AddScoped<IEMailService, EMailService>();

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    options.DefaultRequestCulture = new RequestCulture("tr-TR");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});


builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");

builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { new CultureInfo("tr"), new CultureInfo("en") };
    options.DefaultRequestCulture = new RequestCulture("tr");
    options.SupportedCultures = supportedCultures;
    options.SupportedUICultures = supportedCultures;
});

builder.Services.AddDataProtection()
    .PersistKeysToDbContext<ApplicationDbContext>() // Store keys in database
    .SetApplicationName("SevvalApp");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(GeneralConstants.ConnectionString));





// Sonra Identity'yi ekleyin
builder.Services.AddIdentity<ApplicationUser, Role>(options =>
{
    // Þifre ayarlarý
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequiredLength = 4; // Minimum uzunluk
    options.Password.RequiredUniqueChars = 0;

    // Hata mesajlarýný Türkçe'ye çevir
    options.Password.RequireNonAlphanumeric = false; // Alfanümerik olmayan karakter gereksinimi
    options.Password.RequireLowercase = false; // Küçük harf gereksinimi
    options.Password.RequireUppercase = false; // Büyük harf gereksinimi
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddRoles<Role>()
.AddDefaultTokenProviders();

builder.Services.AddServices(builder.Configuration);

// Session ayarları
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.Name = ".Sevval.Session";
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
    options.Cookie.SameSite = SameSiteMode.Lax;
});



// Harici kimlik doðrulama servisleri
builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.SlidingExpiration = false; // Kullanýcý çýkmadýðý sürece çýkýþ yapmasýn
        options.ExpireTimeSpan = TimeSpan.FromDays(90); // Oturum süresi 90 gün
        options.LoginPath = "/Account/Login"; // Giriþ sayfasýnýn yolu
        options.LogoutPath = "/Account/Logout"; // Çýkýþ sayfasýnýn yolu
        options.AccessDeniedPath = "/Account/AccessDenied";

    })
    .AddGoogle(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];  // Google OAuth ClientId
        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];  // Google OAuth ClientSecret
        options.CallbackPath = "/signin-google";

    })
    .AddApple(options =>
    {
        options.ClientId = builder.Configuration["Authentication:Apple:ClientId"];  // Apple OAuth ClientId
        options.ClientSecret = builder.Configuration["Authentication:Apple:ClientSecret"];  // Apple OAuth ClientSecret
        options.CallbackPath = "/Account/AppleResponse";
        options.SaveTokens = true;
    });


// Session desteði ekle
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10); // Daha makul bir süre önerilir
    options.Cookie.HttpOnly = true; // Cookie'nin JavaScript tarafýndan eriþilmesini engeller
    options.Cookie.IsEssential = true; // Cookie'nin önemli olduðunu belirtir
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Sadece HTTPS üzerinden eriþilebilir
    options.Cookie.SameSite = SameSiteMode.Lax; // Cross-site request'leri engellemek için
});

// Uygulama oluþturuluyor
var app = builder.Build();

// Yerelleþtirme ara yazýlýmýný ekleyin
app.UseRequestLocalization();

// Kullanýcý doðrulama sonrasýnda Claims verilerini eklemek için middleware
app.Use(async (context, next) =>
{
    try
    {
        var user = context.User;

        if (user.Identity.IsAuthenticated)
        {
            var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();

            var appUser = await userManager.GetUserAsync(user);

            if (appUser != null)
            {
                // Kullanýcý bilgilerini Claims'e ekle
                if (!user.HasClaim(c => c.Type == "FirstName"))
                {
                    ((ClaimsIdentity)user.Identity).AddClaim(new Claim("FirstName", appUser.FirstName ?? "Bilinmeyen"));
                    ((ClaimsIdentity)user.Identity).AddClaim(new Claim("LastName", appUser.LastName ?? "Bilinmeyen"));
                    ((ClaimsIdentity)user.Identity).AddClaim(new Claim("PhoneNumber", appUser.PhoneNumber ?? "Bilinmeyen"));
                    ((ClaimsIdentity)user.Identity).AddClaim(new Claim("Email", appUser.Email ?? "Bilinmeyen"));
                }
            }
        }
        await next();
    }
    catch (Exception s)
    {


    }
});

// Veritabanýný oluþturma/kontrol
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();
}

// Hata yönetimi ve HTTPS ayarlarý
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}



app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
// Session yönetimi





app.UseSession();


app.UseAuthentication();
app.UseAuthorization();


// Route ekleme
app.MapControllerRoute(
    name: "ilanver",
    pattern: "ilan/ver",
    defaults: new { controller = "Ilan", action = "IlanVer" }
);

app.MapControllerRoute(
    name: "detay",
    pattern: "ilan/detay/{id?}",
    defaults: new { controller = "Ilan", action = "IlanDetay" }
);


// Route ekleme
app.MapControllerRoute(
    name: "blogDetailsSlug", // Yeni bir rota adı
    pattern: "Blog/{titleSlug}", // SEO dostu URL deseni
    defaults: new { controller = "Blog", action = "DetailsBySlug" } // Yeni bir action'a yönlendireceğiz
);


app.MapControllerRoute(
    name: "kategoriSecim",
    pattern: "Ilan/KategoriSecim",
    defaults: new { controller = "Ilan", action = "KategoriSecim" }
);

// En son varsayılan route
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();
app.MapControllers();

// Uygulama çalýþtýrýlýyor
app.Run();


