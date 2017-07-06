using System;
using System.IO;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using NotificationService.WEB.Infrastructure.Automapper;

namespace NotificationService.IntegrationTests
{
    public class TestBase : IDisposable
    {
        public virtual void Dispose()
        {
        }

        protected IMapper GetMapper()
        {
            var config = new AutoMapperConfiguration();

            return config.Configure().CreateMapper();
        }

        protected Mock<ILogger<T>> GetLoggerMock<T>() where T : class
        {
            return new Mock<ILogger<T>>();
        }

        protected string GetConnectionString()
        {
            var fileName = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
            var jsonFile = File.ReadAllText(fileName);

            var parsedData = JObject.Parse(jsonFile);
            var connectionString = parsedData["ConnectionStrings"].ToString();
            return connectionString;
        }
    }
}