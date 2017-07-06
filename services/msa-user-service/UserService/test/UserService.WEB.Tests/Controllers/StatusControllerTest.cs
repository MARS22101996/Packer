using Microsoft.AspNetCore.Mvc;
using UserService.Tests.Core.Attributes;
using UserService.WEB.Controllers;
using Xunit;

namespace UserService.WEB.Tests.Controllers
{
    [Category(TestType.Unit)]
    public class StatusControllerTest : TestBase
    {
        private readonly StatusController _sut;

        public StatusControllerTest()
        {
            _sut = new StatusController();
        }

        [Fact]
        public void Get_ReturnsOk_Always()
        {
            var result = _sut.Get();

            Assert.IsType<OkResult>(result);
        }
    }
}