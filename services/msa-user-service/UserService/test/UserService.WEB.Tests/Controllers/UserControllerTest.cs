using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserService.BLL.DTO;
using UserService.BLL.Infrastructure.Exceptions;
using UserService.BLL.Interfaces;
using UserService.WEB.Controllers;
using UserService.WEB.Models.AccountApiModels;
using Xunit;
using UserService.Tests.Core.Attributes;

namespace UserService.WEB.Tests.Controllers
{
    [Category(TestType.Unit)]
    public class UserControllerTest : TestBase
    {
        private readonly UserController _sut;
        private readonly Mock<IUserService> _userServiceMock;

        public UserControllerTest()
        {
            var mapper = GetMapper();
            var loggerMock = GetLoggerMock<UserController>();
            _userServiceMock = new Mock<IUserService>();
            _sut = new UserController(mapper, _userServiceMock.Object, loggerMock.Object);
        }

        [Fact]
        public async Task Get_ReturnsJsonWithUser_IfIdIsValid()
        {
            var userId = Guid.NewGuid();

            _userServiceMock
                .Setup(userService => userService.GetAsync(userId))
                .ReturnsAsync(new UserDto());

            var result = await _sut.Get(userId);
            var jsonResult = result as JsonResult;

            Assert.NotNull(jsonResult?.Value as UserApiModel);
        }

        [Fact]
        public async Task Get_ThrowsEntityNotFoundException_IfIdIsInvalid()
        {
            var userId = Guid.NewGuid();

            _userServiceMock
                .Setup(userService => userService.GetAsync(userId))
                .ThrowsAsync(new EntityNotFoundException(string.Empty, string.Empty));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.Get(userId));
        }

        [Fact]
        public void Get_ReturnsAllUsers_IfEmailIsNull()
        {
            var users = new List<UserDto> { new UserDto(), new UserDto() };

            _userServiceMock
                .Setup(userService => userService.GetAll())
                .Returns(users);

            var result = _sut.Get(null);
            var jsonResult = result as JsonResult;

            var userApiModels = jsonResult?.Value as IEnumerable<UserApiModel>;
            Assert.Equal(users.Count, userApiModels?.Count());
        }

        [Fact]
        public void Get_ReturnsOneUser_IfEmailIsValid()
        {
            const string email = "email-stub";

            _userServiceMock
                .Setup(userService => userService.Get(email))
                .Returns(new UserDto());

            var result = _sut.Get(email);
            var jsonResult = result as JsonResult;

            Assert.NotNull(jsonResult?.Value as UserApiModel);
        }

        [Fact]
        public async Task AddToRole_ThrowsEntityNotFoundException_IfArgumentsAreInvalid()
        {
            var user = new UserDto
            {
                Id = Guid.NewGuid()
            };

            _userServiceMock
                .Setup(userService => userService.AddToRoleAsync(user.Id, It.IsAny<string>()))
                .Throws(new EntityNotFoundException(string.Empty, string.Empty));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.AddToRole(user.Id, It.IsAny<string>()));
        }

        [Fact]
        public async Task RemoveFromRole_ThrowsEntityNotFoundException_IfArgumentsAreInvalid()
        {
            var user = new UserDto
            {
                Id = Guid.NewGuid()
            };

            _userServiceMock
                .Setup(userService => userService.RemoveRoleAsync(user.Id, It.IsAny<string>()))
                .Throws(new EntityNotFoundException(string.Empty, string.Empty));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.RemoveFromRole(user.Id, It.IsAny<string>()));
        }
    }
}