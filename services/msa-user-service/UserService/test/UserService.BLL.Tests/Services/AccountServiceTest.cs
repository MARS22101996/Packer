using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using UserService.BLL.DTO;
using UserService.BLL.Infrastructure.Exceptions;
using UserService.BLL.Interfaces;
using UserService.BLL.Services;
using UserService.DAL.Entities;
using UserService.DAL.Interfaces;
using Xunit;
using UserService.Tests.Core.Attributes;

namespace UserService.BLL.Tests.Services
{
    [Category(TestType.Unit)]
    public class AccountServiceTest : TestBase
    {
        private readonly IAccountService _sut;
        private readonly Mock<ICryptoProvider> _cryptoProviderMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public AccountServiceTest()
        {
            var mapper = GetMapper();
            var loggerMock = GetLoggerMock<AccountService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            var roleServiceMock = new Mock<IRoleService>();
            _cryptoProviderMock = new Mock<ICryptoProvider>();
            _sut = new AccountService(
                _unitOfWorkMock.Object, 
                roleServiceMock.Object, 
                _cryptoProviderMock.Object,
                mapper,
                loggerMock.Object);
        }

        [Fact]
        public void Login_ThrowsServiceException_IfUserDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new List<User>().AsQueryable());

            Assert.Throws<ServiceException>(() => _sut.Login(It.IsAny<LoginModelDto>()));
        }

        [Fact]
        public void Login_ThrowsServiceException_IfPasswordIsNotValid()
        {
            var loginModel = new LoginModelDto { Password = "password-stub" };
            var users = new List<User> { new User { PasswordHash = "hash-stub" } };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(users.AsQueryable());

            _cryptoProviderMock
                .Setup(provider => provider.VerifyHash(loginModel.Password, users[0].PasswordHash))
                .Returns(false);

            Assert.Throws<ServiceException>(() => _sut.Login(loginModel));
        }

        [Fact]
        public void Login_ReturnsUser_IfLoginModelIsValid()
        {
            var loginModel = new LoginModelDto { Email = "email-sub", Password = "password-stub" };
            var users = new List<User> { new User { Email = "email-sub", PasswordHash = "hash-stub" } };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(users.AsQueryable());

            _cryptoProviderMock
                .Setup(provider => provider.VerifyHash(loginModel.Password, users[0].PasswordHash))
                .Returns(true);

            var result = _sut.Login(loginModel);

            Assert.Equal(result.Email, users[0].Email);
        }

        [Fact]
        public async Task RegisterAsync_ThrowsEntityExistsException_IfUserWithSpecifiedEmailAlreadyExists()
        {
            var registerModel = new RegisterModelDto { Email = "email-stub", Password = "password-stub" };
            var users = new List<User> { new User() };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(users.AsQueryable());

            await Assert.ThrowsAsync<EntityExistsException>(() => _sut.RegisterAsync(registerModel));
        }

        [Fact]
        public async Task RegisterAsync_CallsCreate_IfRegisterModelIsValid()
        {
            var registerModel = new RegisterModelDto { Email = "email-stub", Password = "password-stub" };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(new List<User>().AsQueryable());

            await _sut.RegisterAsync(registerModel);

            _unitOfWorkMock.Verify(work => work.Users.CreateAsync(It.IsAny<User>()), Times.Once);
        }
    }
}