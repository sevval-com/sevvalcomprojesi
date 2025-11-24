using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Sevval.Domain.Entities;
using Sevval.Persistence.Context;
using sevvalemlak.Models;

namespace sevvalemlak.Filters
{
    public class EIDSVerificationAttribute : TypeFilterAttribute
    {
        public EIDSVerificationAttribute() : base(typeof(EIDSVerificationFilter))
        {
        }

        private class EIDSVerificationFilter : IAsyncActionFilter
        {
            private readonly UserManager<ApplicationUser> _userManager;
            private readonly ApplicationDbContext _context;
            private readonly ILogger<EIDSVerificationAttribute> _logger;

            public EIDSVerificationFilter(
                UserManager<ApplicationUser> userManager,
                ApplicationDbContext context,
                ILogger<EIDSVerificationAttribute> logger)
            {
                _userManager = userManager;
                _context = context;
                _logger = logger;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var durum = context.RouteData.Values["durum"]?.ToString();

                if (durum == "Kiralık" || durum == "Devren Kiralık")
                {
                    var user = await _userManager.GetUserAsync(context.HttpContext.User);
                    if (user == null)
                    {
                        _logger.LogWarning("EIDSVerification: Kullanıcı bulunamadı");
                        context.Result = new RedirectToActionResult("Login", "Account", null);
                        return;
                    }

                    //var activeVerification = await _context.EIDSVerifications
                    //    .Where(v => v.UserId == user.Id &&
                    //           v.IsActive &&
                    //           v.ExpiryDate > DateTime.Now)
                    //    .OrderByDescending(v => v.VerificationDate)
                    //    .FirstOrDefaultAsync();

                    //_logger.LogInformation($"EIDSVerification: UserId: {user.Id}, HasVerification: {activeVerification != null}");

                    //if (activeVerification == null)
                    //{
                    //    context.Result = new RedirectToActionResult("EDevletAuth", "Account", null);
                    //    return;
                    //}

                    //context.HttpContext.Session.SetString("EIDSUserCode", activeVerification.EIDSUserCode);
                    //context.HttpContext.Session.SetString("EIDSVerificationDate", activeVerification.VerificationDate.ToString("O"));



                }

                await next();
            }
        }
    }
}