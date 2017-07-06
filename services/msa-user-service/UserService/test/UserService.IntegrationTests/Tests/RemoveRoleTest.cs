using System;
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
    public class RemoveRoleTest : TestBase
    {
        private readonly RoleController _roleController;

        private Role _databaseRole;

        public RemoveRoleTest()
        {
            var mapper = GetMapper();
            var roleServiceLoggerMock = GetLoggerMock<RoleService>();
            var roleControllerLoggerMock = GetLoggerMock<RoleController>();

            var roleService = new RoleService(UnitOfWork, mapper, roleServiceLoggerMock.Object);

            _roleController = new RoleController(roleService, mapper, roleControllerLoggerMock.Object);
        }

        [Fact]
        public void ShouldRemoveRole()
        {
            this.Given(tests => tests.GivenDatabaseWithSpecifiedRole())
                .Given(tests => tests.WhenSpecifiedRoleIdDeleting())
                .When(tests => tests.ThenThisRoleShouldBeDeletedFromDatabase())
                .BDDfy<AddRemoveRolesStory>();
        }

        public override void Dispose()
        {
            Task.Run(() => UnitOfWork.Roles.DeleteAsync(_databaseRole.Id)).Wait();
        }

        private async Task GivenDatabaseWithSpecifiedRole()
        {
            _databaseRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "stub-remove-role-databse-name"
            };

            await UnitOfWork.Roles.CreateAsync(_databaseRole);
        }

        private async Task WhenSpecifiedRoleIdDeleting()
        {
            await _roleController.Delete(_databaseRole.Id);
        }

        private async Task ThenThisRoleShouldBeDeletedFromDatabase()
        {
            var user = await UnitOfWork.Roles.GetAsync(_databaseRole.Id);

            Assert.Null(user);
        }
    }
}