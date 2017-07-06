using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using NotificationService.BLL.DTO;
using NotificationService.BLL.Infrastructure;
using NotificationService.BLL.Infrastructure.Exceptions;
using NotificationService.BLL.Interfaces;
using NotificationService.Core.Enums;
using NotificationService.DAL.Entities;
using NotificationService.DAL.Interfaces;
using NotificationService.Tests.Core.Attributes;
using Xunit;

namespace NotificationService.BLL.Tests.Services
{
    [Category(TestType.Unit)]
    public class NotificationServiceTest : TestBase
    {
        private readonly INotificationService _sut;
        private readonly Mock<IEmailSender> _emailSenderMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public NotificationServiceTest()
        {
            var mapper = GetMapper();
            var loggerMock = GetLoggerMock<BLL.Services.NotificationService>();
            _emailSenderMock = new Mock<IEmailSender>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new BLL.Services.NotificationService(
                _unitOfWorkMock.Object,
                _emailSenderMock.Object,
                mapper,
                loggerMock.Object);
        }

        [Fact]
        public async Task NotifyTicketWatchersAsync_SendsMailsToAllWatchers_IfDataExistsAndNotificationTypeIsTicketDeleted()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();

            var testWatchers = new List<User>
            {
                new User
                {
                    Email = "email-stub-1"
                },
                new User
                {
                    Email = "email-stub-2"
                }
            };

            _unitOfWorkMock
                .Setup(work => work.Watchers.GetAllAsync(teamId, ticketId))
                .ReturnsAsync(testWatchers);

            var testTicketDto = new TicketDto();

            var testNotificationInfo = new NotificationInfo
            {
               NotificationType = NotificationType.TicketDeleted,
               OldTicket = testTicketDto
            };

            await _sut.NotifyTicketWatchersAsync(teamId, ticketId, testNotificationInfo);

            var emails = new List<string>
            {
                testWatchers[0].Email,
                testWatchers[1].Email
            };

            _emailSenderMock.Verify(
                emailSender => emailSender.SendAsync(emails, It.IsAny<string>(), "TaskManager notification"),
                Times.Once);
        }

        [Fact]
        public async Task NotifyTicketWatchersAsync_SendsMailsToAllWatchers_IfDataExistsAndNotificationTypeIsAssigneeChanged()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();

            var testWatchers = new List<User>
            {
                new User
                {
                    Email = "email-stub-1"
                },
                new User
                {
                    Email = "email-stub-2"
                }
            };

            _unitOfWorkMock
                .Setup(work => work.Watchers.GetAllAsync(teamId, ticketId))
                .ReturnsAsync(testWatchers);

            var testTicketDto = new TicketDto();

            var testNotificationInfo = new NotificationInfo
            {
                NotificationType = NotificationType.AssigneeChanged,
                NewTicket = testTicketDto,
                OldTicket = testTicketDto
            };

            await _sut.NotifyTicketWatchersAsync(teamId, ticketId, testNotificationInfo);

            var emails = new List<string>
            {
                testWatchers[0].Email,
                testWatchers[1].Email
            };

            _emailSenderMock.Verify(
                emailSender => emailSender.SendAsync(emails, It.IsAny<string>(), "TaskManager notification"),
                Times.Once);
        }

        [Fact]
        public async Task NotifyTicketWatchersAsync_SendsMailsToAllWatchers_IfDataExistsAndNotificationTypeIsTicketUpdated()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();

            var testWatchers = new List<User>
            {
                new User
                {
                    Email = "email-stub-1"
                },
                new User
                {
                    Email = "email-stub-2"
                }
            };

            _unitOfWorkMock
                .Setup(work => work.Watchers.GetAllAsync(teamId, ticketId))
                .ReturnsAsync(testWatchers);

            var testTicketDto = new TicketDto();

            var testNotificationInfo = new NotificationInfo
            {
                NotificationType = NotificationType.TicketUpdated,
                NewTicket = testTicketDto,
                OldTicket = testTicketDto
            };

            await _sut.NotifyTicketWatchersAsync(teamId, ticketId, testNotificationInfo);

            var emails = new List<string>
            {
                testWatchers[0].Email,
                testWatchers[1].Email
            };

            _emailSenderMock.Verify(
                emailSender => emailSender.SendAsync(emails, It.IsAny<string>(), "TaskManager notification"), 
                Times.Once);
        }

        [Fact]
        public async Task NotifyTicketWatchersAsync_CallsSendAsyncMethod_IfDataExistsAndNotificationTypeIsStatusUpdated()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();

            var testWatchers = new List<User>
            {
                new User
                {
                    Email = "email-stub-1"
                },
                new User
                {
                    Email = "email-stub-2"
                }
            };

            var testTicketDto = new TicketDto();

            var testNotificationInfo = new NotificationInfo
            {
                NotificationType = NotificationType.StatusUpdated,
                NewTicket = testTicketDto,
                OldTicket = testTicketDto
            };

            _unitOfWorkMock
                .Setup(work => work.Watchers.GetAllAsync(teamId, ticketId))
                .ReturnsAsync(testWatchers);

            await _sut.NotifyTicketWatchersAsync(teamId, ticketId, testNotificationInfo);

            var emails = new List<string>
            {
                testWatchers[0].Email,
                testWatchers[1].Email
            };

            _emailSenderMock.Verify(
                emailSender => emailSender.SendAsync(emails, It.IsAny<string>(), "TaskManager notification"),
                Times.Exactly(1));
        }

        [Fact]
        public async Task GetWatcher_ReturnsCorrectWatcher_IfDataExists()
        {
            var watcherId = Guid.NewGuid();
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();

            var testWatcher = new User
            {
                Id = watcherId,
                Email = "email-stub-1"
            };

            _unitOfWorkMock
                .Setup(work => work.Watchers.GetAsync(teamId, ticketId, watcherId))
                .ReturnsAsync(testWatcher);

            var result = await _sut.GetWatcherAsync(teamId, ticketId, watcherId);

            Assert.Equal(watcherId, result.Id);
        }

        [Fact]
        public async Task GetWatcher_ThrowsEntityNotFoundException_IfWatcherDoesNotExist()
        {
            var watcherId = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(work => work.Watchers.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.GetWatcherAsync(Guid.NewGuid(), Guid.NewGuid(), watcherId));
        }

        [Fact]
        public async Task GetAllWatchers_ReturnsCorrectNumberOfWatchers_IfDataExists()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            var testWatchers = new List<User>
            {
                new User
                {
                    Email = "email-1"
                },
                new User
                {
                    Email = "email-2"
                }
            };

            _unitOfWorkMock
                .Setup(work => work.Watchers.GetAllAsync(teamId, ticketId))
                .ReturnsAsync(testWatchers);

            var result = await _sut.GetAllWatchersAsync(teamId, ticketId);

            Assert.Equal(result.Count(), testWatchers.Count);
        }

        [Fact]
        public async Task GetAllWatchers_ThrowsEntityNotFoundException_IfNotificationsForTicketDoNotExist()
        {
            _unitOfWorkMock
                .Setup(work => work.Watchers.GetAllAsync(It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(new List<User>());

            var result = await _sut.GetAllWatchersAsync(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsType(typeof(List<UserDto>), result);
        }

        [Fact]
        public async Task AddWatcherAsync_CallsCreateAsyncFromDAL()
        {
            var ticketId = Guid.NewGuid();
            var testUser = new UserDto { Id = Guid.NewGuid(), Email = "email-1" };

            _unitOfWorkMock
                .Setup(work => work.Watchers.AddAsync(ticketId, It.IsAny<User>()))
                .ReturnsAsync(ticketId);

            await _sut.AddWatcherAsync(ticketId, testUser);

            _unitOfWorkMock.Verify(work => work.Watchers.AddAsync(ticketId, It.IsAny<User>()), Times.Once);
        }

        [Fact]
        public async Task RemoveWatcherAsync_CallsDeleteAsyncFromDAL_IfIdsAreValid()
        {
            var teamId = Guid.NewGuid();
            var ticketId = Guid.NewGuid();
            var testWatcher = new User
            {
                Id = Guid.NewGuid()
            };

            _unitOfWorkMock
                .Setup(work => work.Watchers.GetAsync(teamId, ticketId, testWatcher.Id))
                .ReturnsAsync(testWatcher);

            await _sut.RemoveWatcherAsync(teamId, ticketId, testWatcher.Id);

            _unitOfWorkMock.Verify(work => work.Watchers.RemoveAsync(ticketId, testWatcher.Id), Times.Once);
        }

        [Fact]
        public async Task RemoveWatcherAsync_ThrowsEntityNotFoundException_IfOneOrBothIdsAreInvalid()
        {
            _unitOfWorkMock
                .Setup(work => work.Watchers.GetAsync(It.IsAny<Guid>(), It.IsAny<Guid>(), It.IsAny<Guid>()))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(
                () => _sut.RemoveWatcherAsync(Guid.NewGuid(), Guid.NewGuid(), Guid.NewGuid()));
        }
    }
}