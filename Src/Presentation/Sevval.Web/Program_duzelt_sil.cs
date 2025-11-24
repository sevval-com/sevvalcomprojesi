//using Microsoft.AspNetCore.DataProtection;
//using Microsoft.AspNetCore.Http.Features;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Localization;
//using Microsoft.AspNetCore.Server.Kestrel.Core;
//using AspNet.Security.OAuth.Apple;
//using Microsoft.EntityFrameworkCore;
//using Sevval.Application.Interfaces.IService;
//using Sevval.Domain.Entities;
//using Sevval.Infrastructure.Services;
//using Sevval.Persistence.Context;
//using sevvalemlak.csproj;
//using System.Globalization;
//using System.Net;
//using System.Net.Mail;
//using System.Security.Claims;
//using Sevval.Application.Constants;
//using Microsoft.Data.Sqlite;

//// ===================================================
//// SSH Namespaces
//using SevvalEmlak.Areas.SSH.Data;
//using SevvalEmlak.Areas.SSH.Helpers;
//using SevvalEmlak.Areas.SSH.Services;

//var builder = WebApplication.CreateBuilder(args);

//// ===================================================
//// CONFIGURATION FILES (Sevval + SSH)
//builder.Configuration
//    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

//// ===================================================
//// REQUEST SIZE LIMITS
//builder.Services.Configure<IISServerOptions>(options =>
//{
//    options.MaxRequestBodySize = 1073741824; // 1 GB
//});
//builder.Services.Configure<KestrelServerOptions>(options =>
//{
//    options.Limits.MaxRequestBodySize = 1073741824; // 1 GB
//});
//builder.Services.Configure<FormOptions>(options =>
//{
//    options.MultipartBodyLengthLimit = 1073741824; // 1 GB
//});

//// ===================================================
//// LOCALIZATION
//var supportedCultures = new List<CultureInfo>
//{
//    new CultureInfo("tr-TR")
//};
//builder.Services.AddLocalization(options => options.ResourcesPath = "Resources");
//builder.Services.Configure<RequestLocalizationOptions>(options =>
//{
//    var supported = new[] { new CultureInfo("tr"), new CultureInfo("en") };
//    options.DefaultRequestCulture = new RequestCulture("tr");
//    options.SupportedCultures = supported;
//    options.SupportedUICultures = supported;
//});

//// ===================================================
//// CONTROLLERS & RAZOR
//builder.Services.AddControllersWithViews()
//    .AddRazorRuntimeCompilation()
//    .AddJsonOptions(opt =>
//    {
//        opt.JsonSerializerOptions.PropertyNamingPolicy = null;
//    });
//builder.Services.AddRazorPages();
//builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
//builder.Services.AddHttpClient();

//// ===================================================
//// SMTP EMAIL SERVICE
//builder.Services.AddScoped<SmtpClient>(serviceProvider =>
//{
//    var configuration = serviceProvider.GetRequiredService<IConfiguration>();
//    return new SmtpClient(configuration["Email:SmtpServer"])
//    {
//        Port = int.Parse(configuration["Email:SmtpPort"]),
//        Credentials = new NetworkCredential(
//            configuration["Email:Username"],
//            configuration["Email:Password"]
//        ),
//        EnableSsl = true
//    };
//});
//builder.Services.AddScoped<IEMailService, EMailService>();

//// ===================================================
//// DATA PROTECTION
//builder.Services.AddDataProtection()
//    .PersistKeysToDbContext<ApplicationDbContext>()
//    .SetApplicationName("SevvalApp");

//// ===================================================
//// DATABASE CONTEXTS
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlite(GeneralConstants.ConnectionString));

//builder.Services.AddDbContext<AppDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("MainConnection")));

//builder.Services.AddDbContext<SSHDbContext>(options =>
//    options.UseSqlite(builder.Configuration.GetConnectionString("SSHConnection")));

//// Set SQLite Data Directory for SSH
//var basePath = AppDomain.CurrentDomain.BaseDirectory;
//AppDomain.CurrentDomain.SetData("DataDirectory", Path.Combine(basePath, "Areas", "SSH", "App_Data"));

//// ===================================================
//// SSH - JSON SERVICE
//builder.Services.AddScoped<ILocationService, LocationService>();

//// ===================================================
//// SSH - CORS
//builder.Services.AddCors(options =>
//{
//    options.AddPolicy("AllowSshSites", policy =>
//    {
//        policy
//            .SetIsOriginAllowed(origin =>
//            {
//                if (string.IsNullOrWhiteSpace(origin)) return false;
//                try
//                {
//                    var uri = new Uri(origin);
//                    var host = uri.Host.ToLowerInvariant();

//                    // Ana proje & SSH domainlerini kapsar
//                    if (host.Contains("sevval.com")) return true;
//                    if (host == "localhost") return true;

//                    return host.EndsWith(".sevval.com");
//                }
//                catch { return false; }
//            })
//            .AllowAnyHeader()
//            .AllowAnyMethod()
//            .AllowCredentials();
//    });
//});

//// ===================================================
//// SSH - SWAGGER
//builder.Services.AddEndpointsApiExplorer();
//builder.Services.AddSwaggerGen();

//// ===================================================
//// IDENTITY & SESSION
//builder.Services.AddIdentity<ApplicationUser, Role>(options =>
//{
//    options.Password.RequireDigit = false;
//    options.Password.RequireLowercase = true;
//    options.Password.RequireNonAlphanumeric = false;
//    options.Password.RequireUppercase = false;
//    options.Password.RequiredLength = 4;
//    options.Password.RequiredUniqueChars = 0;
//})
//.AddEntityFrameworkStores<ApplicationDbContext>()
//.AddRoles<Role>()
//.AddDefaultTokenProviders();

//builder.Services.AddServices(builder.Configuration);

//builder.Services.AddDistributedMemoryCache();
//builder.Services.AddSession(options =>
//{
//    options.IdleTimeout = TimeSpan.FromMinutes(10);
//    options.Cookie.Name = ".Sevval.Session";
//    options.Cookie.HttpOnly = true;
//    options.Cookie.IsEssential = true;
//    options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
//    options.Cookie.SameSite = SameSiteMode.Lax;
//});

//// ===================================================
//// AUTHENTICATION (GOOGLE, APPLE)
//builder.Services.AddAuthentication()
//    .AddCookie(options =>
//    {
//        options.SlidingExpiration = false;
//        options.ExpireTimeSpan = TimeSpan.FromDays(90);
//        options.LoginPath = "/Account/Login";
//        options.LogoutPath = "/Account/Logout";
//        options.AccessDeniedPath = "/Account/AccessDenied";
//    })
//    .AddGoogle(options =>
//    {
//        options.ClientId = builder.Configuration["Authentication:Google:ClientId"];
//        options.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
//        options.CallbackPath = "/signin-google";
//    })
//    .AddApple(options =>
//    {
//        options.ClientId = builder.Configuration["Authentication:Apple:ClientId"];
//        options.ClientSecret = builder.Configuration["Authentication:Apple:ClientSecret"];
//        options.CallbackPath = "/Account/AppleResponse";
//        options.SaveTokens = true;
//    });

//// ===================================================
//// BUILD APPLICATION
//var app = builder.Build();

//// ===================================================
//// SSH - WAL MODE 
//try
//{
//    using var conn = new SqliteConnection(builder.Configuration.GetConnectionString("SSHConnection"));
//    conn.Open();

//    using var cmd = conn.CreateCommand();
//    cmd.CommandText = @"
//        PRAGMA journal_mode = WAL;
//        PRAGMA synchronous = NORMAL;
//        PRAGMA busy_timeout = 5000;
//    ";
//    cmd.ExecuteNonQuery();

//    Console.WriteLine("WAL mode ENABLED (SSH DB)");
//}
//catch (Exception ex)
//{
//    Console.WriteLine("WAL mode FAILED: " + ex.Message);
//}

//// ===================================================
//// SSH - SUPERADMIN SEED 
//try
//{
//    using var scope = app.Services.CreateScope();
//    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
//    SeedHelper.SeedSuperAdmin(context);
//}
//catch (Exception ex)
//{
//    Console.WriteLine("SeedSuperAdmin FAILED: " + ex.Message);
//}

//// ===================================================
//// DATABASE CREATION CHECK 
//try
//{
//    using var scope = app.Services.CreateScope();
//    var services = scope.ServiceProvider;
//    services.GetRequiredService<ApplicationDbContext>().Database.EnsureCreated();
//    services.GetRequiredService<AppDbContext>().Database.EnsureCreated();
//    services.GetRequiredService<SSHDbContext>().Database.EnsureCreated();
//}
//catch (Exception ex)
//{
//    Console.WriteLine("EnsureCreated FAILED: " + ex.Message);
//}

//// ===================================================
//// LOCALIZATION
//app.UseRequestLocalization();

//// ===================================================
//// ERROR / SWAGGER HANDLING
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
//else
//{
//    app.UseExceptionHandler("/Home/Error");
//    app.UseHsts();
//}

//// ===================================================
//// CLAIMS INJECTION
//app.Use(async (context, next) =>
//{
//    try
//    {
//        var user = context.User;
//        if (user.Identity.IsAuthenticated)
//        {
//            var userManager = context.RequestServices.GetRequiredService<UserManager<ApplicationUser>>();
//            var appUser = await userManager.GetUserAsync(user);

//            if (appUser != null && !user.HasClaim(c => c.Type == "FirstName"))
//            {
//                var identity = (ClaimsIdentity)user.Identity;
//                identity.AddClaim(new Claim("FirstName", appUser.FirstName ?? "Bilinmeyen"));
//                identity.AddClaim(new Claim("LastName", appUser.LastName ?? "Bilinmeyen"));
//                identity.AddClaim(new Claim("PhoneNumber", appUser.PhoneNumber ?? "Bilinmeyen"));
//                identity.AddClaim(new Claim("Email", appUser.Email ?? "Bilinmeyen"));
//            }
//        }
//        await next();
//    }
//    catch { await next(); }
//});

//// ===================================================
//// MIDDLEWARE  
//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();

//app.UseCors("AllowSshSites");
//app.UseSession();

//app.UseAuthentication();
//app.UseAuthorization();

//// ===================================================
//// ROUTES
//app.MapControllerRoute(
//    name: "ilanver",
//    pattern: "ilan/ver",
//    defaults: new { controller = "Ilan", action = "IlanVer" }
//);

//app.MapControllerRoute(
//    name: "detay",
//    pattern: "ilan/detay/{id?}",
//    defaults: new { controller = "Ilan", action = "IlanDetay" }
//);

//app.MapControllerRoute(
//    name: "blogDetailsSlug",
//    pattern: "Blog/{titleSlug}",
//    defaults: new { controller = "Blog", action = "DetailsBySlug" }
//);

//app.MapControllerRoute(
//    name: "kategoriSecim",
//    pattern: "Ilan/KategoriSecim",
//    defaults: new { controller = "Ilan", action = "KategoriSecim" }
//);

//// SSH AREAS ROUTE
//app.MapControllerRoute(
//    name: "areas",
//    pattern: "{area:exists}/{controller=Dashboard}/{action=Index}/{id?}"
//);

//// Default route
//app.MapControllerRoute(
//    name: "default",
//    pattern: "{controller=Home}/{action=Index}/{id?}"
//);

//app.MapRazorPages();
//app.MapControllers();

//// ===================================================
//// RUN APP
//app.Run();
