using EpamMA.Communication.Interfaces;
using EpamMA.Communication.Services;
using Microsoft.Extensions.DependencyInjection;
using TicketService.BLL.Infrastructure.DI;
using TicketService.BLL.Interfaces;
using TicketService.BLL.Services;
using Microsoft.Extensions.Configuration;

namespace TicketService.WEB.Infrastructure.DI
{
    public static class DependencyResolver
    {
        public static void Resolve(IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ITicketService, BLL.Services.TicketService>();
            services.AddTransient<ICommentService, CommentService>();
            services.AddTransient<ITagService, TagService>();
            services.AddTransient<ITicketLinkService, TicketLinkService>();
            services.AddTransient<ICommunicationService, CommunicationService>();

            DependencyResolverModule.Configure(services, configuration);
        }
    }
}