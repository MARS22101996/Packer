using System;
using System.IO;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json.Linq;
using UserService.DAL.Context;
using UserService.DAL.Interfaces;
using UserService.DAL.UnitOfWorks;
using UserService.WEB.Infrastructure.Automapper;

namespace UserService.IntegrationTests.Tests
{
    public class TestBase : IDisposable
    {
        protected readonly IUnitOfWork UnitOfWork;
        private const string ConfigPath = "netcoreapp1.0/config.json";
        protected TestBase()
        {
            UnitOfWork = GetUnitOfWork();
        }

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
            var loggerMock = new Mock<ILogger<T>>();

            return loggerMock;
        }

        private UnitOfWork GetUnitOfWork()
        {
            var pathForCi = Path.Combine(Directory.GetCurrentDirectory(), "config.json");
            var json = File.ReadAllText(pathForCi);
            var rss = JObject.Parse(json);
            var connectionString = rss["connectionString"];

            var databaseContext = new DbContext(connectionString.ToString());
            var unitOfWork = new UnitOfWork(databaseContext);

            return unitOfWork;
        }
    }
}