﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationService.BLL.Interfaces;
using NotificationService.DAL.Context;
using NotificationService.DAL.Interfaces;
using NotificationService.DAL.Repositories;
using NotificationService.DAL.UnitOfWorks;
using NotificationService.WEB.Controllers;
using NotificationService.WEB.Models;
using TestStack.BDDfy;
using Xunit;
using NotificationService.Tests.Core.Attributes;
using notificationService = NotificationService.BLL.Services.NotificationService;

namespace NotificationService.IntegrationTests.Controllers
{
    [Category(TestType.Integration)]
    public class GetAllWatchersOfTicket : TestBase
    {
        private readonly NotificationController _sut;
        private readonly IUnitOfWork _unitOfWork;
        private Guid _teamId;
        private Guid _ticketId;
        private UserApiModel _user;

        public GetAllWatchersOfTicket()
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
            .Then(test => test.ThenAddTestWatcherForTicket())
                .And(test => test.AndCheckThatListOfWatchersHasOneElement())
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
            _ticketId = new Guid("a92439d0-efc5-4e44-86ba-a44461f03ee5");
            _teamId = new Guid("a92439d0-efc5-4e44-86ba-a44461f03ee4");

            _user = new UserApiModel
            {
                Id = Guid.NewGuid()
            };
        }

        private async Task ThenAddTestWatcherForTicket()
        {
            await _sut.Post(_teamId, _ticketId, _user);
        }

        private async Task AndCheckThatListOfWatchersHasOneElement()
        {
            var watchers = await _sut.Get(_teamId, _ticketId);
            var result = watchers as OkObjectResult;

            var watchersList = result?.Value as IEnumerable<UserApiModel>;

            Assert.NotNull(watchersList);

            Assert.Equal(1, watchersList.Count());
        }
    }
}