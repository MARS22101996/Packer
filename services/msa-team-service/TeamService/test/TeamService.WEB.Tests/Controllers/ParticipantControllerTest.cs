using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TeamService.BLL.DTO;
using TeamService.BLL.Interfaces;
using TeamService.WEB.Controllers;
using TeamService.WEB.Models;
using Xunit;
using EpamMA.Communication.Interfaces;
using TeamService.Tests.Core.Enums;
using TeamService.Tests.Core.Attributes;

namespace TeamService.WEB.Tests.Controllers
{
    [Category(TestType.Unit)]
    public class ParticipantControllerTest : TestBase
    {
        private readonly ParticipantController _sut;
        private readonly Mock<ITeamService> _teamServiceMock;

        public ParticipantControllerTest()
        {
            var mapper = GetMapper();
            var loggerMock = GetLoggerMock<TeamController>();
            _teamServiceMock = new Mock<ITeamService>();
            var communicationServiceMock = new Mock<ICommunicationService>();
            _sut = new ParticipantController(_teamServiceMock.Object, mapper, loggerMock.Object, communicationServiceMock.Object);
        }

        [Fact]
        public async Task Post_ReturnsOkResult_WhenInputDataIsValid()
        {
            var userId = Guid.NewGuid();

            SetupRequestHeader(_sut, userId);

            _teamServiceMock
              .Setup(teamService => teamService.GetAsync(userId, It.IsAny<Guid>()))
              .ReturnsAsync(new TeamDto { Owner = new UserDto { Id = userId } });

            var result = await _sut.Post(userId, new UserApiModel { Id = Guid.NewGuid() });

            Assert.IsType(typeof(OkResult), result);
        }

        [Fact]
        public async Task Post_ReturnsBadRequestResult_WhenTeamIdIsNull()
        {
            SetupRequestHeader(_sut, Guid.NewGuid());

            var result = await _sut.Post(null, new UserApiModel());

            Assert.IsType(typeof(BadRequestObjectResult), result);
        }

        [Fact]
        public async Task Post_ReturnsBadRequestResult_WhenTeamIdIsNotValid()
        {
            SetupRequestHeader(_sut, Guid.NewGuid());

            var result = await _sut.Post(null, new UserApiModel());

            Assert.IsType(typeof(BadRequestObjectResult), result);
        }

        [Fact]
        public async Task Post_ReturnsBadRequestResult_WhenUserAndTeamIdAreNotValid()
        {
            SetupRequestHeader(_sut, Guid.NewGuid());

            var result = await _sut.Post(null, null);

            Assert.IsType(typeof(BadRequestObjectResult), result);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenInputIsValid()
        {
            SetupRequestHeader(_sut, Guid.NewGuid());

            var result = await _sut.Delete(Guid.NewGuid(), Guid.NewGuid());

            Assert.IsType(typeof(OkResult), result);
        }

        [Fact]
        public async Task UnassignUserFromTeam_ReturnsBadRequestResult_WhenTeamIdIsNotValid()
        {
            SetupRequestHeader(_sut, Guid.NewGuid());

            var result = await _sut.Delete(null, null);

            Assert.IsType(typeof(BadRequestObjectResult), result);
        }
    }
}