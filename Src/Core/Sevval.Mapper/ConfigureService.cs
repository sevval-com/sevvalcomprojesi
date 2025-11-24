
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Sevval.Mapper.Mapper
{
    public static class ConfigureService
    {
        public static void AddMapper(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddSingleton<Application.Interfaces.AutoMapper.IMapper, AutoMapper.Mapper>();
           
            services.AddAutoMapper(assembly);

        }
    }
}
