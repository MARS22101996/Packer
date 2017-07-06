using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TestStack.BDDfy;
using UserService.BLL.Infrastructure.CryptoProviders;
using UserService.BLL.Services;
using UserService.DAL.Entities;
using UserService.WEB.Controllers;
using UserService.WEB.Models.AccountApiModels;
using Xunit;
using UserService.Tests.Core.Attributes;

namespace UserService.IntegrationTests.Tests
{
    [Category(TestType.Integration)]
    public class RegisterTests : TestBase
    {
        private readonly AccountController _accountController;

        private RegisterApiModel _registerApiModel;
        private User _databaseUser;
        private StatusCodeResult _statusCodeResult;

        public RegisterTests()
        {
            var mapper = GetMapper();
            var roleServiceLoggerMock = GetLoggerMock<RoleService>();
            var accauntServiceLoggerMock = GetLoggerMock<AccountService>();
            var accauntControllerLoggerMock = GetLoggerMock<AccountController>();
            var cryptoProvider = new MD5CryptoProvider();

            var roleService = new RoleService(UnitOfWork, mapper, roleServiceLoggerMock.Object);
            var accauntService = new AccountService(
                UnitOfWork,
                roleService,
                cryptoProvider,
                mapper,
                accauntServiceLoggerMock.Object);

            _accountController = new AccountController(accauntService, mapper, accauntControllerLoggerMock.Object);
        }

        [Fact]
        public void ShouldRegisterUser()
        {
            this.Given(tests => tests.GivenUser())
                .When(tests => tests.WhenSpecifiedUserRegisters())
                .Then(tests => AndStatusCodeIsSuccesfull())
                .Then(tests => tests.ThenUserShouldBeCreatedInDatabase())
                .And(tests => AndTheUserShouldHaveTheSameEmail())
                .And(tests => tests.AndTheUserShouldHaveTheSameName())
                .BDDfy<RegisterStory>();
        }

        public override void Dispose()
        {
            var databaseUser = UnitOfWork.Users.Find(user => user.Email.Equals(_registerApiModel.Email)).FirstOrDefault();

            if (databaseUser != null)
            {
                Task.Run(() => UnitOfWork.Users.DeleteAsync(databaseUser.Id)).Wait();
            }
        }

        private void GivenUser()
        {
            const string password = "stub-register-password";

            _registerApiModel = new RegisterApiModel
            {
                Name = "stub-register-name",
                Email = "stub-register@email",
                Password = password,
                ConfirmPassword = password
            };
        }

        private async Task WhenSpecifiedUserRegisters()
        {
            var actionResult = await _accountController.Register(_registerApiModel);
            _statusCodeResult = (StatusCodeResult)actionResult;
        }

        private void AndStatusCodeIsSuccesfull()
        {
            Assert.NotNull(_statusCodeResult);
            Assert.Equal(StatusCodes.Status200OK, _statusCodeResult.StatusCode);
        }

        private void ThenUserShouldBeCreatedInDatabase()
        {
            _databaseUser = UnitOfWork.Users.Find(user => user.Email.Equals(_registerApiModel.Email)).FirstOrDefault();

            Assert.NotNull(_databaseUser);
        }

        private void AndTheUserShouldHaveTheSameEmail()
        {
            Assert.Equal(_registerApiModel.Email, _databaseUser.Email);
        }

        private void AndTheUserShouldHaveTheSameName()
        {
            Assert.Equal(_registerApiModel.Name, _databaseUser.Name);
        }
    }
}