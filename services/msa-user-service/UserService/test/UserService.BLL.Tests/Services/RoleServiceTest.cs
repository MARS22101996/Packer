using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Moq;
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
    public class RoleServiceTest : TestBase
    {
        private readonly IRoleService _sut;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public RoleServiceTest()
        {
            var mapper = GetMapper();
            var loggerMock = GetLoggerMock<RoleService>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new RoleService(_unitOfWorkMock.Object, mapper, loggerMock.Object);
        }

        [Fact]
        public async Task GetAsync_ReturnRole_WhenRoleExists()
        {
            var roleGuid = Guid.NewGuid();

            var role = new Role
            {
                Name = "stub-role",
                Id = roleGuid
            };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Roles.GetAsync(roleGuid))
                .ReturnsAsync(role);

            var result = await _sut.GetAsync(roleGuid);
            
            Assert.Equal(result.Id, roleGuid);
        }

        [Fact]
        public async Task GetAsync_ThrowsEntityNotFoundException_WhenRoleNotExists()
        {
            var roleGuid = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Roles.GetAsync(roleGuid))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.GetAsync(roleGuid));
        }

        [Fact]
        public void Get_ReturnsRole_WhenRoleExists()
        {
            var roleGuid = Guid.NewGuid();

            var role = new Role
            {
                Name = "stub-role",
                Id = roleGuid
            };

            var roles = new List<Role> { role };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Roles.Find(It.IsAny<Expression<Func<Role, bool>>>()))
                .Returns(roles.AsQueryable);

            var result = _sut.Get(role.Name);

            Assert.Equal(result.Id, roleGuid);
        }

        [Fact]
        public void Get_ThrowsEntityNotFoundException_WhenRoleDoesNotExist()
        {
            const string roleName = "stub-role";

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Roles.Find(It.IsAny<Expression<Func<Role, bool>>>()))
                .Returns(new List<Role>().AsQueryable);

            Assert.Throws<EntityNotFoundException>(() => _sut.Get(roleName));
        }

        [Fact]
        public void GetAll_ReturnsCorrectNumberOfRoles_IfDataExists()
        {
            var roles = new List<Role> { new Role(), new Role() };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Roles.GetAll())
                .Returns(roles.AsQueryable());

            var result = _sut.GetAll();

            Assert.Equal(roles.Count, result.Count());
        }

        [Fact]
        public async Task CreateAsync_CallsCreateAsyncFromDal_IfRoleDataIsValid()
        {
            const string roleName = "stub-name";

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Roles.Find(It.IsAny<Expression<Func<Role, bool>>>()))
                .Returns(new List<Role>().AsQueryable());

            await _sut.CreateAsync(roleName);

            _unitOfWorkMock.Verify(work => work.Roles.CreateAsync(It.IsAny<Role>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_ThrowsEntityExistsException_IfRoleWithSpecifiedNameAlreadyExists()
        {
            var roleName = "stub-name";
            var roles = new List<Role> { new Role() };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Roles.Find(It.IsAny<Expression<Func<Role, bool>>>()))
                .Returns(roles.AsQueryable());

            await Assert.ThrowsAsync<EntityExistsException>(() => _sut.CreateAsync(roleName));
        }

        [Fact]
        public async Task DeleteAsync_ThrowsEntityNotFoundException_IfThereIsNoSuchRole()
        {
            var roleGuid = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Roles.GetAsync(roleGuid))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.DeleteAsync(roleGuid));
        }

        [Fact]
        public async Task DeleteAsync_CallDeleteAsyncFromDal_IfRoleDataIsValid()
        {
            var roleGuid = Guid.NewGuid();
            var role = new Role();

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Roles.GetAsync(roleGuid))
                .ReturnsAsync(role);

            await _sut.DeleteAsync(roleGuid);

            _unitOfWorkMock.Verify(work => work.Roles.DeleteAsync(roleGuid), Times.Once);
        }
    }
}