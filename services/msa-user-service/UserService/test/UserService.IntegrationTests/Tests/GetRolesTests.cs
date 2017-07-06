using System;
using System.Collections.Generic;
using System.Linq;
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
    public class GetRolesTests : TestBase
    {
        private readonly RoleController _roleController;

        private Role _databaseRole;
        private RoleApiModel _roleApiModel;

        private List<Role> _databaseRoles;
        private List<RoleApiModel> _roleApiModels;

        public GetRolesTests()
        {
            var mapper = GetMapper();
            var roleServiceLoggerMock = GetLoggerMock<RoleService>();
            var roleControllerLoggerMock = GetLoggerMock<RoleController>();

            var roleService = new RoleService(UnitOfWork, mapper, roleServiceLoggerMock.Object);

            _roleController = new RoleController(roleService, mapper, roleControllerLoggerMock.Object);
        }

        [Fact]
        public void ExecuteGetRoleById()
        {
            this.Given(tests => tests.GivenDatabaseWithSpecifiedRole())
                .When(tests => tests.WhenSpecifiedRoleRequestedById())
                .Then(tests => tests.ThenReturnedRoleShouldHaveTheSameName())
                .BDDfy<GetRoleStory>();
        }

        [Fact]
        public void ExecuteGetAllRoles()
        {
            this.Given(tests => tests.GivenDatabaseWithSpecifiedRoles())
                .When(tests => tests.WhenAllRolesRequested())
                .Then(tests => tests.ThenAllRollsShouldBeReturned())
                .BDDfy<GetRoleStory>();
        }

        public override void Dispose()
        {
            if (_databaseRoles != null)
            {
                foreach (var role in _databaseRoles)
                {
                    Task.Run(() => UnitOfWork.Roles.DeleteAsync(role.Id)).Wait();
                }
            }

            if (_databaseRole != null)
            {
                Task.Run(() => UnitOfWork.Roles.DeleteAsync(_databaseRole.Id)).Wait();
            }
        }

        private async Task GivenDatabaseWithSpecifiedRole()
        {
            _databaseRole = new Role
            {
                Id = Guid.NewGuid(),
                Name = "stub-get-role-name-1"
            };

            await UnitOfWork.Roles.CreateAsync(_databaseRole);
        }

        private async Task WhenSpecifiedRoleRequestedById()
        {
            var actionResult = await _roleController.Get(_databaseRole.Id);
            var jsonResult = (JsonResult)actionResult;
            _roleApiModel = (RoleApiModel)jsonResult.Value;
        }

        private void ThenReturnedRoleShouldHaveTheSameName()
        {
            Assert.Equal(_databaseRole.Name, _roleApiModel.Name);
        }

        private async Task GivenDatabaseWithSpecifiedRoles()
        {
            _databaseRoles = new List<Role>
            {
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "stub-get-role-name-2"
                },
                new Role
                {
                    Id = Guid.NewGuid(),
                    Name = "stub-get-role-name-3"
                }
            };
            
            await UnitOfWork.Roles.CreateAsync(_databaseRoles[0]);
            await UnitOfWork.Roles.CreateAsync(_databaseRoles[1]);
        }

        private void WhenAllRolesRequested()
        {
            var actionResult = _roleController.Get();
            var jsonResult = (JsonResult)actionResult;
            _roleApiModels = (List<RoleApiModel>)jsonResult.Value;
        }

        private void ThenAllRollsShouldBeReturned()
        {
            Assert.NotEmpty(_roleApiModels);

            Assert.Collection(
                _databaseRoles, 
                role => Assert.Contains(role.Name, _roleApiModels.Select(apiModel=>apiModel.Name)),
                role => Assert.Contains(role.Name, _roleApiModels.Select(apiModel => apiModel.Name)));
        }
    }
}