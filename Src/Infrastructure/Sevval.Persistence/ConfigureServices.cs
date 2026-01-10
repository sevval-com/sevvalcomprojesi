

using GridBox.Solar.Domain.IRepositories;
using GridBox.Solar.Domain.IUnitOfWork;
using Microsoft.Extensions.DependencyInjection;
using Sevval.Application.Interfaces.Messaging;
using Sevval.Persistence.Repositories;
using Sevval.Persistence.Repositories.Messaging;
using Sevval.Persistence.UnitOfWorks;

namespace Sevval.Persistence
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services)
        {
            services.AddScoped(typeof(IReadRepository<>), typeof(ReadRepository<>));
            services.AddScoped(typeof(IWriteRepository<>), typeof(WriteRepository<>));
            services.AddScoped(typeof(IUnitOfWork), typeof(UnitOfWork));
            services.AddScoped<IMessageReadRepository, MessageRepository>();
            services.AddScoped<IMessageWriteRepository, MessageRepository>();
            services.AddScoped<IMessageReadStateRepository, MessageReadStateRepository>();

            return services;
        }
    }

}
