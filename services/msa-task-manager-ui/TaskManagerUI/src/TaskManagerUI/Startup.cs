using System;
using System.Text;
using AutoMapper;
using EpamMA.Communication.Infrastructure;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskManagerUI.Filters;
using TaskManagerUI.Infrastructure.Authorization;
using TaskManagerUI.Infrastructure.DI;

namespace TaskManagerUI
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            DependencyResolver.Resolve(services, Configuration);

            services
                .Configure<TokenValidationParameters>(Configuration.GetSection("TokenValidationParameters"))
                .Configure<TokenValidationParameters>(options =>
                {
                    var signingKey = new SymmetricSecurityKey(
                    Encoding.ASCII.GetBytes(Configuration["TokenValidationParameters:SecretKey"]));
                    options.IssuerSigningKey = signingKey;
                });

            services.Configure<CommunicationOptions>(Configuration.GetSection("CommunicationOptions"));

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ErrorFilter));
            });
            services.AddAutoMapper();
        }

        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env, 
            ILoggerFactory loggerFactory,
            IOptions<TokenValidationParameters> tokenValidationOptions)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseAuthorizationMiddleware();
            app.UseStaticFiles();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationOptions.Value
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Ticket}/{action=Board}/{id?}");
            });
        }
    }
}