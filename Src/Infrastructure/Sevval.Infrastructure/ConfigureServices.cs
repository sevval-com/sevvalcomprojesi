using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sevval.Application.Abstractions.Services;
using Sevval.Application.Interfaces.IService;
using Sevval.Application.Interfaces.IService.Common;
using Sevval.Application.Interfaces.Services;
using Sevval.Infrastructure.Services;
using System.Net;
using System.Net.Mail;


namespace Sevval.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {


        services.AddScoped<IDateTimeService, DateTimeService>();

        services.AddScoped<IDistrictService, DistrictService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEMailService, EMailService>();
        services.AddScoped<IContractService, ContractService>();
        services.AddScoped<IProvinceService, ProvinceService>();
        services.AddScoped<INeighbourhoodService, NeighbourhoodService>();
        services.AddScoped<IRoleService, RoleService>();
        services.AddScoped<ITempEstateVerificationService, TempEstateVerificationService>();
        services.AddScoped<IVisitorService, VisitorService>();
        services.AddScoped<IAnnouncementService, AnnouncementService>();
        services.AddScoped<IWhatsAppService, WhatsAppService>();
        services.AddScoped<IAdvertisingService, AdvertisingService>();
        services.AddScoped<IPropertyStatusService, PropertyStatusService>();
        services.AddScoped<IPropertyTypeService, PropertyTypeService>();
        services.AddScoped<ILandStatusService, LandStatusService>();
        services.AddScoped<ILandTypeService, LandTypeService>();
        services.AddScoped<IGardenStatusService, GardenStatusService>();
        services.AddScoped<IGardenTypeService, GardenTypeService>();
        services.AddScoped<IFieldStatusService, FieldStatusService>();
        services.AddScoped<IFieldTypeService, FieldTypeService>();
        services.AddScoped<IBusinessStatusService, BusinessStatusService>();
        services.AddScoped<IBusinessTypeService, BusinessTypeService>();
        services.AddScoped<IFacilityTypeService, FacilityTypeService>();
        services.AddScoped<IFacilityStatusService, FacilityStatusService>();
        services.AddScoped<IRoomOptionsService, RoomOptionsService>();
        services.AddScoped<IBuildingAgeService, BuildingAgeService>();
        services.AddScoped<IFloorOptionsService, FloorOptionsService>();
        services.AddScoped<IRecentlyVisitedAnnouncementService, RecentlyVisitedAnnouncementService>();
        services.AddScoped<IBathroomOptionsService, BathroomOptionsService>();
        services.AddScoped<IHeatingSystemOptionsService, HeatingSystemOptionsService>();
        services.AddScoped<IBalconyOptionsService, BalconyOptionsService>();
        services.AddScoped<ISlopeOptionsService, SlopeOptionsService>();
        services.AddScoped<IRoadConditionOptionsService, RoadConditionOptionsService>();
        services.AddScoped<IDistanceToSettlementOptionsService, DistanceToSettlementOptionsService>();
        services.AddScoped<IZoningStatusOptionsService, ZoningStatusOptionsService>();
        services.AddScoped<IRecentlyVisitedAnnouncementService, RecentlyVisitedAnnouncementService>();
        services.AddScoped<ISalesRequestService, SalesRequestService>();
        services.AddScoped<IInvestmentRequestService, InvestmentRequestService>();
        services.AddScoped<IVideoService, VideoService>();
        services.AddScoped<IAboutService, AboutService>();
        services.AddScoped<IContactInfoService, ContactInfoService>();
        services.AddScoped<ISocialMediaService, SocialMediaService>();
        services.AddScoped<ICompanyService, CompanyService>();
        services.AddScoped<IConsultantService, ConsultantService>();
        services.AddScoped<IAboutUsService, AboutUsService>();
        services.AddScoped<IFavoriteService, FavoriteService>();

        services.AddScoped<SmtpClient>(serviceProvider =>

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
        services.AddScoped<IEMailService, EMailService>();

        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

        // services.Configure<RedisCacheSettings>(configuration.GetSection("RedisCacheSettings"));
        //services.AddScoped<IRedisCacheService, RedisCacheService>();

        /*services.AddStackExchangeRedisCache(opt =>
        {
            opt.Configuration = configuration["RedisCacheSettings:ConnectionString"];
            opt.InstanceName = configuration["RedisCacheSettings:InstanceName"];
        });*/

        return services;
    }

}
