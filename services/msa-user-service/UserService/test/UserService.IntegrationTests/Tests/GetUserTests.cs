using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using TestStack.BDDfy;
using UserService.BLL.Services;
using UserService.DAL.Entities;
using UserService.WEB.Controllers;
using UserService.WEB.Models.AccountApiModels;
using Xunit;
using UserService.Tests.Core.Attributes;

namespace UserService.IntegrationTests.Tests
{
    [Category(TestType.Integration)]
    public class GetUserTests : TestBase
    {
        private readonly UserController _userController;

        private User _databaseUser;
        private UserApiModel _userApiModel;

        public GetUserTests()
        {
            var mapper = GetMapper();
            var roleServiceLoggerMock = GetLoggerMock<RoleService>();
            var userServiceLoggerMock = GetLoggerMock<BLL.Services.UserService>();
            var userControllerLoggerMock = GetLoggerMock<UserController>();

            var roleService = new RoleService(UnitOfWork, mapper, roleServiceLoggerMock.Object);
            var userService = new BLL.Services.UserService(UnitOfWork, roleService, mapper, userServiceLoggerMock.Object);

            _userController = new UserController(mapper, userService, userControllerLoggerMock.Object);
        }

        [Fact]
        public void ShouldGetUserByEmail()
        {
            this.Given(s => s.GivenDatabseWithSpecifiedUser())
                .When(s => s.WhenSpecifiedUserRequestedByEmail())
                .Then(s => s.ThenreturnedUserShouldHaveTheSameEmail())
                .And(s => s.AndReturnedUserShouldHaveTheSameName())
                .And(s => s.AndReturnedUserShouldHaveTheSameId())
                .And(s => s.AndReturnedUserShouldHaveTheSameRoles())
                .BDDfy<GetUserStory>();
        }

        [Fact]
        public void ShouldGetUserById()
        {
            this.Given(s => s.GivenDatabseWithSpecifiedUser())
                .When(s => s.WhenSpecifiedUserRequestedById())
                .Then(s => s.ThenreturnedUserShouldHaveTheSameEmail())
                .And(s => s.AndReturnedUserShouldHaveTheSameName())
                .And(s => s.AndReturnedUserShouldHaveTheSameId())
                .And(s => s.AndReturnedUserShouldHaveTheSameRoles())
                .BDDfy<GetUserStory>();
        }

        public override void Dispose()
        {
            Task.Run(() => UnitOfWork.Users.DeleteAsync(_databaseUser.Id)).Wait();
        }

        private async Task GivenDatabseWithSpecifiedUser()
        {
            _databaseUser = new User
            {
                Email = "stub-get-uesr@email",
                Id = Guid.NewGuid(),
                Name = "stub-get-user-name",
                Roles = new List<string> { "stub-get-uesr-role1", "stub-get-user-role2" }
            };

            await UnitOfWork.Users.CreateAsync(_databaseUser);
        }

        private void WhenSpecifiedUserRequestedByEmail()
        {
            var actionResult = _userController.Get(_databaseUser.Email);
            var jsonResult = (JsonResult)actionResult;
            _userApiModel = (UserApiModel)jsonResult.Value;
        }

        private async Task WhenSpecifiedUserRequestedById()
        {
            var actionResult = await _userController.Get(_databaseUser.Id);
            var jsonResult = (JsonResult)actionResult;
            _userApiModel = (UserApiModel)jsonResult.Value;
        }

        private void ThenreturnedUserShouldHaveTheSameEmail()
        {
            Assert.Equal(_databaseUser.Email, _userApiModel.Email);
        }

        private void AndReturnedUserShouldHaveTheSameName()
        {
            Assert.Equal(_databaseUser.Name, _userApiModel.Name);
        }

        private void AndReturnedUserShouldHaveTheSameId()
        {
            Assert.Equal(_databaseUser.Id, _userApiModel.Id);
        }

        private void AndReturnedUserShouldHaveTheSameRoles()
        {
            Assert.Collection(
                _userApiModel.Roles,
                role => Assert.Contains(role, _databaseUser.Roles),
                role => Assert.Contains(role, _databaseUser.Roles));
        }
    }
}