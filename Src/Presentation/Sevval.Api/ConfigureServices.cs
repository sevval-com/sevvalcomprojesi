using Microsoft.Extensions.DependencyInjection;
using Sevval.Application.Interfaces.IService.Common;
using Microsoft.EntityFrameworkCore;
using Sevval.Persistence.Context;
using Microsoft.Extensions.Configuration;
using Sevval.Api.Services;

namespace Sevval.Api;

public static class ConfigureServices
{
    public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();

        services.AddScoped<ICurrentUserService, CurrentUserService>();//httpcontextaccessor singleton olmasından kaynaklı single yapıldı
     
        services.AddDbContext(configuration);

        return services;
    }

    public static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();

      
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseSqlite(configuration.GetConnectionString("DefaultConnection"),
                    builder =>
                    {
                        builder.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
                        builder.CommandTimeout(3000);
                    }));

        services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
        services.AddScoped<ApplicationDbContext>();

        services.AddScoped<AppContextInitializer>();

        return services;
    }
}
