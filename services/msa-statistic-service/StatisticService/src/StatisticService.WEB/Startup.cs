using System;
using System.IO;
using System.Text;
using AutoMapper;
using LoggingNuget.Module;
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
using StatisticService.WEB.Filters;
using StatisticService.WEB.Infrastructure.DI;
using Swashbuckle.AspNetCore.Swagger;
using IConfiguration = Microsoft.Extensions.Configuration.IConfiguration;
using EpamMA.Communication.Infrastructure;
using EpamMA.CircuitBreaker.Models;
using EpamMA.CircuitBreaker;

namespace StatisticService.WEB
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

            services.Configure<CommunicationOptions>(Configuration.GetSection("CommunicationOptions"));

            services
            .Configure<CircuitBreakerOptions>(Configuration.GetSection("CircuitBreakerOptions"));

            services.AddSingleton<IConfiguration>(Configuration);

            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ErrorFilter));
            });

            services.AddAutoMapper();
            services.AddLogging();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Version = "v1", Title = "Swashbuckle Sample API" });
                c.IncludeXmlComments(GetXmlCommentsPath(PlatformServices.Default.Application));
            });

            services
                .Configure<TokenValidationParameters>(Configuration.GetSection("TokenValidationParameters"))
                .Configure<TokenValidationParameters>(options =>
                {
                    var signingKey = new SymmetricSecurityKey(
                        Encoding.ASCII.GetBytes(Configuration["TokenValidationParameters:SecretKey"]));
                    options.IssuerSigningKey = signingKey;
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

            ConfigureLogManager();

            app.UseServiceCorrelationId();

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            app.UseStaticFiles();

            app.UseCircuitBreaker(
            circuitBreakerOptions.Value.ExceptionsAllowedBeforeBreaking,
            TimeSpan.FromSeconds(circuitBreakerOptions.Value.DurationOfBreakInSeconds),
            loggerFactory.CreateLogger<CircuitBreakerMiddleware>());


            app.UseJwtBearerAuthentication(new JwtBearerOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                TokenValidationParameters = tokenValidationOptions.Value
            });

            app.UseMvc();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "V1 Docs");
            });
        }

        private static void ConfigureLogManager()
        {
            LogManager.Configuration.Variables["configDir"] = Path.Combine(Directory.GetCurrentDirectory(), "Logs");
        }

        private string GetXmlCommentsPath(ApplicationEnvironment appEnvironment)
        {
            return Path.Combine(appEnvironment.ApplicationBasePath, $"{appEnvironment.ApplicationName}.xml");
        }
    }
}