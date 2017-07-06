using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;
using UserService.BLL.Services;
using UserService.DAL.Entities;
using UserService.Tests.Core.Attributes;
using UserService.WEB.Controllers;
using Xunit;
using BllUserService = UserService.BLL.Services.UserService;

namespace UserService.IntegrationTests.Tests
{
    [Category(TestType.Integration)]
    public class CreateUserRoleTest : TestBase
    {
        private readonly UserController _userController;

        private User _databaseUser;
        private Role _databaseRole;

        public CreateUserRoleTest()
        {
            var mapper = GetMapper();
            var roleServiceLoggerMock = GetLoggerMock<RoleService>();
            var userServiceLoggerMock = GetLoggerMock<BllUserService>();
            var userControllerLoggerMock = GetLoggerMock<UserController>();
            var roleService = new RoleService(UnitOfWork, mapper, roleServiceLoggerMock.Object);
            var userService = new BllUserService(UnitOfWork, roleService, mapper, userServiceLoggerMock.Object);

            _userController = new UserController(mapper, userService, userControllerLoggerMock.Object);
        }

        [Fact]
        public void ShouldAddRoleToUser()
        {
            this.Given(s => s.GivenDatabseWithSpecifiedUser())
                .Given(s => s.GivenDatabseWithSpecifiedRole())
                .When(s => s.WhenNewRoleAddedToUser())
                .Then(s => s.ThenUserInDatabaseShouldHaveThisRole())
                .BDDfy<AddRemoveUserRoleStory>();
        }

        public override void Dispose()
        {
            Task.Run(() => UnitOfWork.Users.DeleteAsync(_databaseUser.Id)).Wait();
            Task.Run(() => UnitOfWork.Roles.DeleteAsync(_databaseRole.Id)).Wait();
        }

        private async Task GivenDatabseWithSpecifiedUser()
        {
            _databaseUser = new User
            {
                Email = "stub-create-user-role1@mail",
                Id = Guid.NewGuid(),
                Name = "stub-create-user-role-name",
                Roles = new List<string> { "stub-user-role1", "stub-user-role2" }
            };

            await UnitOfWork.Users.CreateAsync(_databaseUser);
        }

        private async Task GivenDatabseWithSpecifiedRole()
        {
            _databaseRole = new Role
            {
                Name = "stub-create-user-role-name",
            };

            await UnitOfWork.Roles.CreateAsync(_databaseRole);
        }

        private async Task WhenNewRoleAddedToUser()
        {
            await _userController.AddToRole(_databaseUser.Id, _databaseRole.Name);
        }

        private async Task ThenUserInDatabaseShouldHaveThisRole()
        {
            var user = await UnitOfWork.Users.GetAsync(_databaseUser.Id);

            Assert.Contains(_databaseRole.Name, user.Roles);
        }
    }
}