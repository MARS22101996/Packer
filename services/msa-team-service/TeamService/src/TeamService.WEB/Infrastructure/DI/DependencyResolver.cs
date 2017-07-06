using EpamMA.Communication.Interfaces;
using EpamMA.Communication.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeamService.BLL.Infrastructure.DI;
using TeamService.BLL.Interfaces;

namespace TeamService.WEB.Infrastructure.DI
{
    public static class DependencyResolver
    {
        public static void Resolve(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(configuration);
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddTransient<ITeamService, BLL.Services.TeamService>();
            services.AddTransient<ICommunicationService, CommunicationService>();

            DependencyResolverModule.Configure(services, configuration);
        }
    }
}