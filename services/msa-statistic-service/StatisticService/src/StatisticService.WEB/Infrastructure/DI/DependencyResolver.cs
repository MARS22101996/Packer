using EpamMA.Communication.Interfaces;
using EpamMA.Communication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StatisticService.BLL.Interfaces;

namespace StatisticService.WEB.Infrastructure.DI
{
    public static class DependencyResolver
    {
        public static void Resolve(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<IStatisticService, BLL.Services.StatisticService>();
            services.AddTransient<ICommunicationService, CommunicationService>();
        }
    }
}