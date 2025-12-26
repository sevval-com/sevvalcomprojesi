using Sevval.Application.Interfaces.IService.Common;
using Sevval.Persistence;
using sevvalemlak.csproj.ClientServices.AnnouncementService;
using sevvalemlak.csproj.ClientServices.CompanyService;
using sevvalemlak.csproj.ClientServices.ConsultantService;
using sevvalemlak.csproj.ClientServices.RecentlyVisitedAnnouncement;
using sevvalemlak.csproj.ClientServices.SalesRequestService;
using sevvalemlak.csproj.ClientServices.SuitableAnnouncements;
using sevvalemlak.csproj.ClientServices.UserServices;
using sevvalemlak.csproj.ClientServices.VisitoryServices;
using sevvalemlak.csproj.Services;
using sevvalemlak.Services;

namespace sevvalemlak.csproj;

public static class ConfigureServices
{
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddHttpClient();

        services.AddScoped<IUserClientService, UserClientService>();
        services.AddScoped<IVisitoryClientService, VisitoryClientService>();
        services.AddScoped<IAnnouncementClientService, AnnouncementClientService>();
        services.AddScoped<INetGsmService, NetGsmService>();
        services.AddScoped<IRecentlyVisitedAnnouncementClientService, RecentlyVisitedAnnouncementClientService>();
        services.AddScoped<ISuitableAnnouncementsClientService, SuitableAnnouncementsClientService>();
        services.AddScoped<IConsultantClientService, ConsultantClientService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<ISalesRequestClientService, SalesRequestClientService>();
        services.AddScoped<ICompanyClientService, CompanyClientService>();

        services.AddPersistence();


        return services;
    }
}
