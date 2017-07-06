using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TicketService.DAL.Context;
using TicketService.DAL.Interfaces;
using TicketService.DAL.UnitOfWorks;

namespace TicketService.BLL.Infrastructure.DI
{
    public class DependencyResolverModule
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            var connectionstring = configuration["ConnectionStrings:MongoDb"];

            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<IDbContext>(provider => new DbContext(connectionstring));
        }
    }
}