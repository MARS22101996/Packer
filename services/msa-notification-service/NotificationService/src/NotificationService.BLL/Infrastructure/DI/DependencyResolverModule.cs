using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.DAL.Context;
using NotificationService.DAL.Interfaces;
using NotificationService.DAL.Repositories;
using NotificationService.DAL.UnitOfWorks;

namespace NotificationService.BLL.Infrastructure.DI
{
    public class DependencyResolverModule
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var connectionstring = configuration["ConnectionStrings:MongoDb"];

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IDbContext>(provider => new DbContext(connectionstring));
            services.AddTransient<IWatcherRepository, WatcherRepository>();
        }
    }
}