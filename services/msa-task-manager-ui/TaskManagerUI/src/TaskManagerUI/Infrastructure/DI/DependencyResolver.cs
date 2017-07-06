using EpamMA.Communication.Interfaces;
using EpamMA.Communication.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace TaskManagerUI.Infrastructure.DI
{
    public static class DependencyResolver
    {
        public static void Resolve(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ICommunicationService, CommunicationService>();
        }
    }
}
