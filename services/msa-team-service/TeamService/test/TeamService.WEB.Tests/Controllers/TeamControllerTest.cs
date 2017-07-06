using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TeamService.BLL.DTO;
using TeamService.BLL.Infrastructure.Exceptions;
using TeamService.BLL.Interfaces;
using TeamService.WEB.Controllers;
using TeamService.WEB.Models;
using Xunit;
using TeamService.Tests.Core.Attributes;
using TeamService.Tests.Core.Enums;
using EpamMA.Communication.Interfaces;

namespace TeamService.WEB.Tests.Controllers
{
    [Category(TestType.Unit)]
    public class TeamControllerTest : TestBase
    {
        private readonly TeamController _sut;
        private readonly Mock<ITeamService> _teamServiceMock;

        public TeamControllerTest()
        {
            var mapper = GetMapper();
            var loggerMock = GetLoggerMock<TeamController>();
            _teamServiceMock = new Mock<ITeamService>();
            var communicationServiceMock = new Mock<ICommunicationService>();
            _sut = new TeamController(_teamServiceMock.Object, mapper, loggerMock.Object, communicationServiceMock.Object);
        }

        [Fact]
        public async Task GetById_ReturnsJsonWithTeam_WhenIdIsValid()
        {
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            SetupRequestHeader(_sut, userId);

            _teamServiceMock
                .Setup(teamService => teamService.GetAsync(userId, teamId))
                .ReturnsAsync(new TeamDto());

            var result = await _sut.GetById(teamId);
            var jsonResult = result as JsonResult;

            Assert.NotNull(jsonResult?.Value as TeamApiModel);
        }

        [Fact]
        public async Task GetById_ThrowsEntityNotFoundException_WhenIdIsInvalid()
        {
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            SetupRequestHeader(_sut, userId);

            _teamServiceMock
                .Setup(teamService => teamService.GetAsync(userId, teamId))
                .ThrowsAsync(new EntityNotFoundException(string.Empty, string.Empty));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.GetById(teamId));
        }

        [Fact]
        public async Task GetById_ReturnsBadrequest_WhenIdIsNull()
        {
            var result = await _sut.GetById(null);

            Assert.IsType<BadRequestObjectResult>(result);
        }

        [Fact]
        public async Task Post_CallsCreateMethod_WhenModelIsValid()
        {
            var userId = Guid.NewGuid();

            SetupRequestHeader(_sut, userId);

            await _sut.Post(new TeamApiModel { Owner = new UserApiModel { Id = userId } });

            _teamServiceMock.Verify(teamService => teamService.CreateAsync(userId, It.IsAny<TeamDto>()), Times.Once);
        }

        [Fact]
        public async Task Post_ReturnsOkResult_WhenModelIsValid()
        {
            var userId = Guid.NewGuid();

            SetupRequestHeader(_sut, userId);

            var model = new TeamApiModel { Owner = new UserApiModel { Id = userId } };

            var result = await _sut.Post(model);

            Assert.IsType(typeof(OkObjectResult), result);
        }

        [Fact]
        public async Task Post_ReturnsBadRequestResult_WhenModelIsNotValid()
        {
            _sut.ModelState.AddModelError("key", "error message");

            var result = await _sut.Post(null);

            Assert.IsType(typeof(BadRequestObjectResult), result);
        }

        [Fact]
        public async Task Put_ReturnsBadRequestResult_WhenModelIsNotValid()
        {
            _sut.ModelState.AddModelError("key", "error message");

            var result = await _sut.Put(null);

            Assert.IsType(typeof(BadRequestObjectResult), result);
        }

        [Fact]
        public async Task Put_ReturnsOkResult_WhenModelIsValid()
        {
            var userId = Guid.NewGuid();

            SetupRequestHeader(_sut, userId);

            var model = new TeamApiModel { Owner = new UserApiModel { Id = userId } };

            var result = await _sut.Put(model);

            Assert.IsType(typeof(OkResult), result);
        }

        [Fact]
        public async Task Delete_CallsDeleteMethod_WhenIdIsValid()
        {
            var userId = Guid.NewGuid();

            var teamId = Guid.NewGuid();

            SetupRequestHeader(_sut, userId);

            _teamServiceMock
                .Setup(teamService => teamService.GetAsync(userId, teamId))
                .ReturnsAsync(new TeamDto { Owner = new UserDto { Id = userId } });

            await _sut.Delete(teamId);

            _teamServiceMock.Verify(teamService => teamService.DeleteAsync(userId, teamId), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenIdIsValid()
        {
            var userId = Guid.NewGuid();

            SetupRequestHeader(_sut, userId);

            var teamId = Guid.NewGuid();

            _teamServiceMock
              .Setup(teamService => teamService.GetAsync(userId, teamId))
              .ReturnsAsync(new TeamDto { Owner = new UserDto { Id = userId } });

            var result = await _sut.Delete(teamId);

            Assert.IsType(typeof(OkResult), result);
        }

        [Fact]
        public async Task Delete_ThrowsEntityNotFoundException_WhenIdIsInvalid()
        {
            var userId = Guid.NewGuid();

            SetupRequestHeader(_sut, userId);

            var teamId = Guid.NewGuid();

            _teamServiceMock
                .Setup(teamService => teamService.GetAsync(userId, teamId))
                .ReturnsAsync(new TeamDto { Owner = new UserDto { Id = userId } });

            _teamServiceMock
                .Setup(teamService => teamService.DeleteAsync(userId, teamId))
                .Throws(new EntityNotFoundException(string.Empty, string.Empty));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.Delete(teamId));
        }
    }
}