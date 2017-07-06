using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Moq;
using UserService.BLL.DTO;
using UserService.BLL.Interfaces;
using UserService.WEB.Controllers;
using UserService.WEB.Models.AccountApiModels;
using Xunit;
using UserService.Tests.Core.Attributes;

namespace UserService.WEB.Tests.Controllers
{
    [Category(TestType.Unit)]
    public class AccountControllerTest : TestBase
    {
        private readonly AccountController _sut;
        private readonly Mock<IAccountService> _accountServiceMock;

        public AccountControllerTest()
        {
            var mapper = GetMapper();
            var loggerMock = GetLoggerMock<AccountController>();
            _accountServiceMock = new Mock<IAccountService>();
           _sut = new AccountController(_accountServiceMock.Object, mapper, loggerMock.Object);
        }

        [Fact]
        public async Task Register_CallsRegisterMethod_WhenModelIsValid()
        {
            // todo move it to _sut.Register
            var model = new RegisterApiModel();

            await _sut.Register(model);

            _accountServiceMock.Verify(accountService => accountService.RegisterAsync(It.IsAny<RegisterModelDto>()), Times.Once);
        }

        [Fact]
        public async Task Register_ReturnsOkResult_WhenModelIsValid()
        {
            // todo move it to _sut.Register
            var model = new RegisterApiModel();

            var result = await _sut.Register(model);

            Assert.IsType(typeof(OkResult), result);
        }

        [Fact]
        public async Task Register_ReturnsBadRequestResult_WhenModelIsNotValid()
        {
            _sut.ModelState.AddModelError("key", "error message");

            var result = await _sut.Register(null);

            Assert.IsType(typeof(BadRequestObjectResult), result);
        }
    }
}