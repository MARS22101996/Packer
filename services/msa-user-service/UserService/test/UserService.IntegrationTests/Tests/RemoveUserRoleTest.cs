using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TestStack.BDDfy;
using UserService.BLL.Services;
using UserService.DAL.Entities;
using UserService.Tests.Core.Attributes;
using UserService.WEB.Controllers;
using Xunit;

namespace UserService.IntegrationTests.Tests
{
    [Category(TestType.Integration)]
    public class RemoveUserRoleTest : TestBase
    {
        private readonly UserController _userController;

        private User _databaseUser;
        private Role _databaseRole;

        public RemoveUserRoleTest()
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
        public void ShouldRemoveRoleFromUser()
        {
            this.Given(s => s.GivenDatabseWithSpecifiedRole())
                .Given(s => s.GivenDatabaseWithSpecifiedUserWithSpecifiedRole())
                .When(s => s.WhenSpecifiedRoleDeletedFromUser())
                .Then(s => s.ThenUserInDatabaseShouldNotHaveThisRole())
                .BDDfy<AddRemoveRolesStory>();
        }

        public override void Dispose()
        {
            Task.Run(() => UnitOfWork.Users.DeleteAsync(_databaseUser.Id)).Wait();
            Task.Run(() => UnitOfWork.Roles.DeleteAsync(_databaseRole.Id)).Wait();
        }

        private async Task GivenDatabseWithSpecifiedRole()
        {
            _databaseRole = new Role
            {
                Name = "stub-user-delete-role-name",
            };

            await UnitOfWork.Roles.CreateAsync(_databaseRole);
        }

        private async Task GivenDatabaseWithSpecifiedUserWithSpecifiedRole()
        {
            _databaseUser = new User
            {
                Email = "stub-user-role2@email",
                Id = Guid.NewGuid(),
                Name = "stub-user-role-name2",
                Roles = new List<string> { "stub-user-role3", "stub-user-role4", _databaseRole.Name }
            };

            await UnitOfWork.Users.CreateAsync(_databaseUser);
        }

        private async Task WhenSpecifiedRoleDeletedFromUser()
        {
            await _userController.RemoveFromRole(_databaseUser.Id, _databaseRole.Name);
        }

        private async Task ThenUserInDatabaseShouldNotHaveThisRole()
        {
            var user = await UnitOfWork.Users.GetAsync(_databaseUser.Id);

            Assert.DoesNotContain(_databaseRole.Name, user.Roles);
        }
    }
}