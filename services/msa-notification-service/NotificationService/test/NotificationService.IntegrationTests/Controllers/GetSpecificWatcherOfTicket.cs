using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationService.BLL.Interfaces;
using NotificationService.DAL.Context;
using NotificationService.DAL.Interfaces;
using NotificationService.DAL.Repositories;
using NotificationService.DAL.UnitOfWorks;
using NotificationService.Tests.Core.Attributes;
using NotificationService.WEB.Controllers;
using NotificationService.WEB.Models;
using TestStack.BDDfy;
using Xunit;
using notificationService = NotificationService.BLL.Services.NotificationService;

namespace NotificationService.IntegrationTests.Controllers
{
    [Category(TestType.Integration)]
    public class GetSpecificWatcherOfTicket : TestBase
    {
        private readonly NotificationController _sut;
        private readonly IUnitOfWork _unitOfWork;
        private Guid _teamId;
        private Guid _ticketId;
        private UserApiModel _user;

        public GetSpecificWatcherOfTicket()
        {
            var mapper = GetMapper();
            var loggerMock = GetLoggerMock<notificationService>();
            var loggerSut = GetLoggerMock<NotificationController>();
            var repository = new WatcherRepository(new DbContext(GetConnectionString()));
            _unitOfWork = new UnitOfWork(repository);
            var emailSender = new Mock<IEmailSender>();
            var notificationService = new notificationService(_unitOfWork, emailSender.Object, mapper, loggerMock.Object);
            _sut = new NotificationController(notificationService, mapper, loggerSut.Object);
        }

        [Fact]
        public void Execute()
        {
            this.Given(test => test.UserForTests())
            .Then(test => test.ThenAddTestWatcher())
                .And(test => test.AndCheckThatWatcherOfThisTicketExists())
            .BDDfy();
        }

        public override void Dispose()
        {
            var watcher = Task.Run(() => _unitOfWork.Watchers.GetAsync(_teamId, _ticketId, _user.Id)).Result;

            if (watcher != null)
            {
                Task.Run(() => _unitOfWork.Watchers.RemoveAsync(_ticketId, watcher.Id)).Wait();
            }
        }

        private void UserForTests()
        {
            _ticketId = new Guid("a92439d0-efc5-4e44-86ba-a44461f03ee3");
            _teamId = new Guid("a92439d0-efc5-4e44-86ba-a44461f03ee2");

            _user = new UserApiModel
            {
                Id = Guid.NewGuid()
            };
        }

        private async Task ThenAddTestWatcher()
        {
            await _sut.Post(_teamId, _ticketId, _user);
        }

        private async Task AndCheckThatWatcherOfThisTicketExists()
        {
            var watcherFromDb = await _sut.Get(_teamId, _ticketId, _user.Id);
            var result = watcherFromDb as OkObjectResult;

            var watcher = result?.Value as UserApiModel;

            Assert.NotNull(watcher);

            Assert.Equal(_user.Id, watcher.Id);
        }
    }
}