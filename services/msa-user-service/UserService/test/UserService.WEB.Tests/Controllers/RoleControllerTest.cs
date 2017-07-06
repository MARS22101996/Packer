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
    public class RoleControllerTest : TestBase
    {
        private readonly RoleController _sut;
        private readonly Mock<IRoleService> _roleServiceMock;

        public RoleControllerTest()
        {
            var mapper = GetMapper();
            var loggerMock = GetLoggerMock<RoleController>();
            _roleServiceMock = new Mock<IRoleService>();
            _sut = new RoleController(_roleServiceMock.Object, mapper, loggerMock.Object);
        }

        [Fact]
        public async Task Get_ReturnsJsonWithRole_IfIdIsValid()
        {
            var roleId = Guid.NewGuid();

            _roleServiceMock
                .Setup(roleService => roleService.GetAsync(roleId))
                .ReturnsAsync(new RoleDto());

            var result = await _sut.Get(roleId);
            var jsonResult = result as JsonResult;

            Assert.NotNull(jsonResult?.Value as RoleApiModel);
        }

        [Fact]
        public async Task Get_ThrowsEntityNotFoundException_IfIdIsInvalid()
        {
            var roleId = Guid.NewGuid();

            _roleServiceMock
                .Setup(roleService => roleService.GetAsync(roleId))
                .ThrowsAsync(new EntityNotFoundException(string.Empty, string.Empty));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.Get(roleId));
        }

        [Fact]
        public void Get_ReturnsAllUsers_IfDataExists()
        {
            var role = new List<RoleDto> { new RoleDto(), new RoleDto() };

            _roleServiceMock
                .Setup(roleService => roleService.GetAll())
                .Returns(role);

            var result = _sut.Get();
            var jsonResult = result as JsonResult;

            Assert.Equal(2, (jsonResult?.Value as IEnumerable<RoleApiModel>)?.Count());
        }

        [Fact]
        public async Task Post_CallsCreateMethod_WhenModelIsValid()
        {
            var model = new RoleApiModel { Name = "role-stub" };

            await _sut.Post(model);

            _roleServiceMock.Verify(accountService => accountService.CreateAsync(model.Name), Times.Once);
        }

        [Fact]
        public async Task Post_ReturnsOkResult_WhenModelIsValid()
        {
            var model = new RoleApiModel();

            var result = await _sut.Post(model);

            Assert.IsType(typeof(OkResult), result);
        }

        [Fact]
        public async Task Post_ReturnsBadRequestResult_WhenModelIsNotValid()
        {
            _sut.ModelState.AddModelError("key", "error message");

            var result = await _sut.Post(null);

            Assert.IsType(typeof(BadRequestObjectResult), result);
        }

        [Fact]
        public async Task Delete_CallsDeleteMethod_WhenIdIsValid()
        {
            var roleId = Guid.NewGuid();

            await _sut.Delete(roleId);

            _roleServiceMock.Verify(accountService => accountService.DeleteAsync(roleId), Times.Once);
        }

        [Fact]
        public async Task Delete_ReturnsOkResult_WhenIdIsValid()
        {
            var roleId = Guid.NewGuid();

            var result = await _sut.Delete(roleId);

            Assert.IsType(typeof(OkResult), result);
        }

        [Fact]
        public async Task Delete_ThrowsEntityNotFoundException_WhenIdIsInvalid()
        {
            var roleId = Guid.NewGuid();

            _roleServiceMock
                .Setup(roleService => roleService.DeleteAsync(roleId))
                .Throws(new EntityNotFoundException(string.Empty, string.Empty));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.Delete(roleId));
        }
    }
}