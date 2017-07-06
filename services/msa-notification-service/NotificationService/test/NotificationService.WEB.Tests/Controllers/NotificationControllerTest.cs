using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using NotificationService.BLL.DTO;
using NotificationService.BLL.Infrastructure;
using NotificationService.BLL.Infrastructure.Exceptions;
using NotificationService.BLL.Interfaces;
using NotificationService.Tests.Core.Attributes;
using NotificationService.WEB.Controllers;
using NotificationService.WEB.Models;
using Xunit;

namespace NotificationService.WEB.Tests.Controllers
{
    [Category(TestType.Unit)]
    public class NotificationControllerTest : TestBase
    {
        private readonly Mock<INotificationService> _notificationServiceMock;
        private readonly NotificationController _sut;

        public NotificationControllerTest()
        {
            var mapper = GetMapper();
            var loggerMock = GetLoggerMock<NotificationController>();
            _notificationServiceMock = new Mock<INotificationService>();
            _sut = new NotificationController(_notificationServiceMock.Object, mapper, loggerMock.Object);
        }

        [Fact]
        public async Task GetAll_ReturnsCorrectNumberOfWatchers_IfDataExists()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            var testWatchers = new List<UserDto> { new UserDto(), new UserDto() };

            _notificationServiceMock
                .Setup(service => service.GetAllWatchersAsync(teamId, ticketId))
                .ReturnsAsync(testWatchers);

            var result = await _sut.Get(teamId, ticketId);
            var watchers = (result as OkObjectResult)?.Value as List<UserApiModel>;

            Assert.Equal(watchers?.Count, testWatchers.Count);
        }

        [Fact]
        public async Task Get_ReturnsCorrectWatcher_IfIdsAreValid()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            var watcherId = Guid.NewGuid();
            var testWatcher = new UserDto { Id = Guid.NewGuid() };

            _notificationServiceMock
                .Setup(service => service.GetWatcherAsync(teamId, ticketId, watcherId))
                .ReturnsAsync(testWatcher);

            var result = await _sut.Get(teamId, ticketId, watcherId);
            var watcher = (result as OkObjectResult)?.Value as UserApiModel;

            Assert.Equal(watcher?.Id, testWatcher.Id);
        }

        [Fact]
        public async Task Get_ThrowsEntityNotFoundException_IfOneOrBothIdsAreInvalid()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            var watcherId = Guid.NewGuid();

            _notificationServiceMock
                .Setup(service => service.GetWatcherAsync(teamId, ticketId, watcherId))
                .ThrowsAsync(new EntityNotFoundException(string.Empty, string.Empty));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.Get(teamId, ticketId, watcherId));
        }

        [Fact]
        public async Task Post_CallsAddWatcherAsync_IfModelIsValid()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();

            _notificationServiceMock
                .Setup(service => service.AddWatcherAsync(ticketId, It.IsAny<UserDto>()))
                .Returns(Task.CompletedTask);

            await _sut.Post(teamId, ticketId, new UserApiModel());

            _notificationServiceMock.Verify(service => service.AddWatcherAsync(ticketId, It.IsAny<UserDto>()));
        }

        [Fact]
        public async Task Post_ReturnsBadRequestObjectResult_IfModelIsInvalid()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();

            _sut.ModelState.AddModelError(string.Empty, string.Empty);
            var result = await _sut.Post(teamId, ticketId, new UserApiModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Delete_CallsRemoveWatcherAsync_IfIdIsValid()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            var watcherId = Guid.NewGuid();

            _notificationServiceMock
                .Setup(service => service.RemoveWatcherAsync(teamId, ticketId, watcherId))
                .Returns(Task.CompletedTask);

            await _sut.Delete(teamId, ticketId, watcherId);

            _notificationServiceMock.Verify(service => service.RemoveWatcherAsync(teamId, ticketId, watcherId));
        }

        [Fact]
        public async Task Delete_ThrowsEntityNotFoundException_IfOneOrBothIdsAreInvalid()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            var watcherId = Guid.NewGuid();

            _notificationServiceMock
                .Setup(service => service.RemoveWatcherAsync(teamId, ticketId, watcherId))
                .Throws(new EntityNotFoundException(string.Empty, string.Empty));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.Delete(teamId, ticketId, watcherId));
        }

        [Fact]
        public async Task Notify_NotifyTicketWatchersAsync_IfModelIsValid()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();

            _notificationServiceMock
                .Setup(service => service.NotifyTicketWatchersAsync(teamId, ticketId, It.IsAny<NotificationInfo>()))
                .Returns(Task.CompletedTask);

            await _sut.Notify(teamId, ticketId, new NotificationInfoApiModel());

            _notificationServiceMock
                .Verify(service => service.NotifyTicketWatchersAsync(teamId, ticketId, It.IsAny<NotificationInfo>()));
        }

        [Fact]
        public async Task Notify_ReturnsBadRequestObjectResult_IfModelIsInvalid()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();

            _sut.ModelState.AddModelError(string.Empty, string.Empty);
            var result = await _sut.Notify(teamId, ticketId, new NotificationInfoApiModel());

            Assert.IsType<BadRequestObjectResult>(result);
        }
    }
}