using System;
﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TeamService.DAL.Context;
using TeamService.DAL.Interfaces;
using TeamService.DAL.UnitOfWorks;

namespace TeamService.BLL.Infrastructure.DI
{
    public class DependencyResolverModule
    {
        public static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            /*var hostIp = Environment.GetEnvironmentVariable("IpMachine");

            configuration["ConnectionStrings:MongoDb"] = "mongodb://"+hostIp+":27017/TeamsDb";*/

            var connectionstring = configuration["ConnectionStrings:MongoDb"];

            services.AddTransient<IUnitOfWork, UnitOfWork>();

            services.AddTransient<IDbContext>(provider => new DbContext(connectionstring));
        }
    }
}
