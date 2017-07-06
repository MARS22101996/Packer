using System;
using System.IO;
using System.Text;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.IdentityModel.Tokens;
using NLog;
using NLog.Extensions.Logging;
using NLog.Web;
using Swashbuckle.AspNetCore.Swagger;
using TeamService.WEB.Filters;
using TeamService.WEB.Infrastructure.DI;
using EpamMA.CircuitBreaker.Models;
using EpamMA.Communication.Infrastructure;
using EpamMA.CircuitBreaker;
using EpamMA.LoggingLayer.Module;

namespace TeamService.WEB
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            env.ConfigureNLog("NLog.config");
        }

        public IConfigurationRoot Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            DependencyResolver.Resolve(services, Configuration);

            services
                .Configure<TokenValidationParameters>(Configuration.GetSection("TokenValidationParameters"))
                .Configure<TokenValidationParameters>(options =>
                {
                    var keyBytes = Encoding.ASCII.GetBytes(Configuration["TokenValidationParameters:SecretKey"]);
                    var signingKey = new SymmetricSecurityKey(keyBytes);

                    options.IssuerSigningKey = signingKey;
                });

            services
                .Configure<CommunicationOptions>(Configuration.GetSection("CommunicationOptions"));

             /*var hostIp = Environment.GetEnvironmentVariable("IpMachine");

             var configurationCommunication = Configuration.GetSection("CommunicationOptions");

             configurationCommunication["HostAddress"] = "http://" + hostIp + ":9999/";

             services.Configure<CommunicationOptions>(configurationCommunication);*/

             /*services.Configure<CommunicationOptions>(myOptions =>
                {
                    myOptions.DefaultPrefix = "UserService";
                    myOptions.HostAddress = "http://"+ hostIp + ":9999/";
                    myOptions.RetryTimeSpans = new [] {0,1};
                }
              );*/



            services
                .Configure<CircuitBreakerOptions>(Configuration.GetSection("CircuitBreakerOptions"));

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ErrorFilter));
            });

            services.AddAutoMapper();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Version = "v1", Title = "User Service API" });
                c.IncludeXmlComments(GetXmlCommentsPath(PlatformServices.Default.Application));
            });
        }

        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory,
            IOptions<TokenValidationParameters> tokenValidationOptions,
            IOptions<CircuitBreakerOptions> circuitBreakerOptions)
        {
            loggerFactory.AddNLog();
            app.AddNLogWeb();

            app.UseCircuitBreaker(
                circuitBreakerOptions.Value.ExceptionsAllowedBeforeBreaking,
                TimeSpan.FromSeconds(circuitBreakerOptions.Value.DurationOfBreakInSeconds),
                loggerFactory.CreateLogger<CircuitBreakerMiddleware>());

            ConfigLogManager();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStaticFiles();

            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationOptions.Value
            });

            app.UseMvc();
            app.UseServiceCorrelationId();
            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
            });
        }

        private string GetXmlCommentsPath(ApplicationEnvironment appEnvironment)
        {
            return Path.Combine(appEnvironment.ApplicationBasePath, $"{appEnvironment.ApplicationName}.xml");
        }

        private void ConfigLogManager()
        {
            LogManager.Configuration.Variables["configDir"] = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        }
    }
}
