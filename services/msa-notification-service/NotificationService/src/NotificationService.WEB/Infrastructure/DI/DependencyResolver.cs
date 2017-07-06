using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NotificationService.BLL.Infrastructure;
using NotificationService.BLL.Infrastructure.DI;
using NotificationService.BLL.Interfaces;

namespace NotificationService.WEB.Infrastructure.DI
{
    public static class DependencyResolver
    {
        public static void Resolve(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddTransient<INotificationService, BLL.Services.NotificationService>();
            services.AddTransient<IEmailSender, EmailSender>();

            DependencyResolverModule.Configure(services, configuration);
        }
    }
}