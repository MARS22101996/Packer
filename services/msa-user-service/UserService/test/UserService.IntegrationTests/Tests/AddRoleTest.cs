using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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
    public class AddRoleTest : TestBase
    {
        private readonly RoleController _roleController;

        private RoleApiModel _roleApiModel;
        private Role _databaseRole;
        private StatusCodeResult _statusCodeResult;

        public AddRoleTest()
        {
            var mapper = GetMapper();
            var roleServiceLoggerMock = GetLoggerMock<RoleService>();
            var roleControllerLoggerMock = GetLoggerMock<RoleController>();

            var roleService = new RoleService(UnitOfWork, mapper, roleServiceLoggerMock.Object);

            _roleController = new RoleController(roleService, mapper, roleControllerLoggerMock.Object);
        }

        [Fact]
        public void ShouldAddRole()
        {
            this.Given(tests => tests.GivenRole())
                .When(tests => tests.WhenNewRoleIsCreating())
                .When(tests => tests.AndStatusCodeIsSuccesfull())
                .Then(tests => tests.ThenTheRoleShouldBeCreatedInDatabase())
                .And(tests => tests.AndTheRoleShouldHaveTheSameName())
                .BDDfy<AddRemoveRolesStory>();
        }

        public override void Dispose()
        {
            var role = UnitOfWork.Roles.Find(dbRole => dbRole.Name.Equals(_roleApiModel.Name)).FirstOrDefault();

            if (role != null)
            {
                Task.Run(() => UnitOfWork.Roles.DeleteAsync(role.Id)).Wait();
            }
        }

        private void GivenRole()
        {
            _roleApiModel = new RoleApiModel
            {
                Name = "stub-create-delete-user-name"
            };
        }

        private async Task WhenNewRoleIsCreating()
        {
            var actionResult = await _roleController.Post(_roleApiModel);
            _statusCodeResult = (StatusCodeResult)actionResult;
        }

        private void AndStatusCodeIsSuccesfull()
        {
            Assert.NotNull(_statusCodeResult);
            Assert.Equal(StatusCodes.Status200OK, _statusCodeResult.StatusCode);
        }

        private void ThenTheRoleShouldBeCreatedInDatabase()
        {
            _databaseRole = UnitOfWork.Roles.Find(role => role.Name.Equals(_roleApiModel.Name)).FirstOrDefault();

            Assert.NotNull(_databaseRole);
        }

        private void AndTheRoleShouldHaveTheSameName()
        {
            Assert.Equal(_roleApiModel.Name, _databaseRole.Name);
        }
    }
}