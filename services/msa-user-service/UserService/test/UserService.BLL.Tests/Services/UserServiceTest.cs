using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
using UserService.BLL.DTO;
using UserService.BLL.Infrastructure.Exceptions;
using UserService.BLL.Interfaces;
using UserService.DAL.Entities;
using UserService.DAL.Interfaces;
using Xunit;
using UserService.Tests.Core.Attributes;

namespace UserService.BLL.Tests.Services
{
    [Category(TestType.Unit)]
    public class UserServiceTest : TestBase
    {
        private readonly IUserService _sut;
        private readonly Mock<IRoleService> _roleServiceMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public UserServiceTest()
        {
            var mapper = GetMapper();
            var loggerMock = GetLoggerMock<BLL.Services.UserService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _roleServiceMock = new Mock<IRoleService>();
            _sut = new BLL.Services.UserService(_unitOfWorkMock.Object, _roleServiceMock.Object, mapper, loggerMock.Object);
        }

        [Fact]
        public void Get_ReturnsUser_WhenUserExists()
        {
            const string userEmail = "fullname@mail.com";

            var user = new User { Email = userEmail };
            var users = new List<User> { user };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(users.AsQueryable());

            var result = _sut.Get(userEmail);

            Assert.Equal(user.Email, result.Email);
        }

        [Fact]
        public void Get_ThrowsEntityNotFoundException_WhenUserNotExists()
        {
            var users = new List<User>().AsQueryable();

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(users);

            Assert.Throws<EntityNotFoundException>(() => _sut.Get(It.IsAny<string>()));
        }

        [Fact]
        public void GetAll_ReturnsCorrectNumberOfUsers_IfDataExists()
        {
            var users = new List<User> { new User(), new User() };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.GetAll())
                .Returns(users.AsQueryable());

            var result = _sut.GetAll();

            Assert.Equal(users.Count, result.Count());
        }

        [Fact]
        public async Task GetAsync_ThrowsEntityNotFoundException_IfUserWithGuidNotExists()
        {
            var users = new List<User>().AsQueryable();

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.Find(It.IsAny<Expression<Func<User, bool>>>()))
                .Returns(users);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.GetAsync(It.IsAny<Guid>()));
        }

        [Fact]
        public async Task GetAsync_ReturnsUser_WhenUserWithGuidExists()
        {
            var user = new User();

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.GetAsync(It.IsAny<Guid>())).ReturnsAsync(user);

            var result = await _sut.GetAsync(It.IsAny<Guid>());

            Assert.IsType<UserDto>(result);
        }

        [Fact]
        public async Task AddToRoleAsync_ThrowsEntityNotFoundException_IfUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.GetAsync(userId))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.AddToRoleAsync(userId, It.IsAny<string>()));
        }

        [Fact]
        public async Task AddToRoleAsync_ThrowsEntityNotFoundException_IfRoleDoesNotExist()
        {
            const string role = "role-stub";
            var userId = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.GetAsync(userId))
                .ReturnsAsync(new User());

            _roleServiceMock
                .Setup(service => service.Get(role))
                .Throws(new EntityNotFoundException(string.Empty, string.Empty));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.AddToRoleAsync(userId, role));
        }

        [Fact]
        public async Task AddToRoleAsync_CallsUpdate_IfArgumentsAreValid()
        {
            const string role = "role-stub";
            var user = new User { Id = Guid.NewGuid() };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.GetAsync(user.Id))
                .ReturnsAsync(user);

            _roleServiceMock
                .Setup(service => service.Get(role))
                .Returns(new RoleDto());

            await _sut.AddToRoleAsync(user.Id, role);

            _unitOfWorkMock.Verify(work => work.Users.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task AddToRoleAsync_AddsRoleToUser_IfArgumentsAreValid()
        {
            const string role = "role-stub";
            var user = new User { Id = Guid.NewGuid() };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.GetAsync(user.Id))
                .ReturnsAsync(user);

            _roleServiceMock
                .Setup(service => service.Get(role))
                .Returns(new RoleDto());

            await _sut.AddToRoleAsync(user.Id, role);

            Assert.NotEmpty(user.Roles);
        }

        [Fact]
        public async Task RemoveRoleAsync_ThrowsEntityNotFoundException_IfUserDoesNotExist()
        {
            var userId = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.GetAsync(userId))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.RemoveRoleAsync(userId, It.IsAny<string>()));
        }

        [Fact]
        public async Task RemoveRoleAsync_ThrowsEntityNotFoundException_IfRoleDoesNotExist()
        {
            const string role = "role-stub";
            var userId = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.GetAsync(userId))
                .ReturnsAsync(new User());

            _roleServiceMock
                .Setup(service => service.Get(role))
                .Throws(new EntityNotFoundException(string.Empty, string.Empty));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.RemoveRoleAsync(userId, role));
        }

        [Fact]
        public async Task RemoveRoleAsync_CallsUpdate_IfArgumentsAreValid()
        {
            const string role = "role-stub";
            var user = new User { Id = Guid.NewGuid(), Roles = new List<string> { role } };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.GetAsync(user.Id))
                .ReturnsAsync(user);

            _roleServiceMock
                .Setup(service => service.Get(role))
                .Returns(new RoleDto());

            await _sut.RemoveRoleAsync(user.Id, role);

            _unitOfWorkMock.Verify(work => work.Users.UpdateAsync(user), Times.Once);
        }

        [Fact]
        public async Task RemoveRoleAsync_RemovesRoleFromUser_IfArgumentsAreValid()
        {
            const string role = "role-stub";
            var user = new User { Id = Guid.NewGuid(), Roles = new List<string> { role } };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.GetAsync(user.Id))
                .ReturnsAsync(user);

            _roleServiceMock
                .Setup(service => service.Get(role))
                .Returns(new RoleDto());

            await _sut.RemoveRoleAsync(user.Id, role);

            Assert.Empty(user.Roles);
        }

        [Fact]
        public async Task IsInRoleAsync_ReturnsTrueIfUserHasRole_IfArgumentsAreValid()
        {
            const string role = "role-stub";
            var user = new User { Id = Guid.NewGuid(), Roles = new List<string> { role } };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.GetAsync(user.Id))
                .ReturnsAsync(user);

            var result = await _sut.IsInRoleAsync(user.Id, role);

            Assert.True(result);
        }

        [Fact]
        public async Task IsInRoleAsync_ReturnsFalseIfUserHasRole_IfArgumentsAreValid()
        {
            const string role = "role-stub";
            var user = new User
            {
                Id = Guid.NewGuid(),
                Roles = new List<string> { "another-role-stub" }
            };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Users.GetAsync(user.Id))
                .ReturnsAsync(user);

            var result = await _sut.IsInRoleAsync(user.Id, role);

            Assert.False(result);
        }
    }
}