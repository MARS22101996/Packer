using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Moq;
using TeamService.BLL.DTO;
using TeamService.BLL.Infrastructure.Exceptions;
using TeamService.BLL.Interfaces;
using TeamService.DAL.Entities;
using TeamService.DAL.Interfaces;
using Xunit;
using TeamService.Tests.Core.Attributes;
using TeamService.Tests.Core.Enums;

namespace TeamService.BLL.Tests.Services
{
    [Category(TestType.Unit)]
    public class TeamServiceTest : TestBase
    {
        private readonly ITeamService _sut;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;

        public TeamServiceTest()
        {
            var mapper = GetMapper();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _sut = new BLL.Services.TeamService(_unitOfWorkMock.Object, mapper);
        }

        [Fact]
        public async Task GetAsync_ReturnsCorrectTeam_WhenTeamExists()
        {
            var team = new Team { Id = Guid.NewGuid(), Owner = new User { Id = Guid.NewGuid() } };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Teams.GetAsync(team.Id))
                .ReturnsAsync(team);

            var result = await _sut.GetAsync(team.Owner.Id, team.Id);

            Assert.Equal(team.Id, result.Id);
        }

        [Fact]
        public async Task GetAsync_ThrowsEntityNotFoundException_WhenTeamNotExists()
        {
            var team = new Team { Id = Guid.NewGuid(), Owner = new User { Id = Guid.NewGuid() } };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Teams.GetAsync(team.Id))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.GetAsync(team.Owner.Id, team.Id));
        }

        [Fact]
        public void CreateAsync_CallsCreateAsyncFromDal_WhenTeamIsValid()
        {
            var model = new TeamDto { Owner = new UserDto { Id = Guid.NewGuid() } };

            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Teams.CreateAsync(It.IsAny<Team>()));

            _sut.CreateAsync(model.Owner.Id, model);

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Teams.CreateAsync(It.IsAny<Team>()), Times.Once);
        }

        [Fact] public async Task CreateAsync_ThrowsNullReferenceException_WhenTeamIsNull()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => _sut.CreateAsync(It.IsAny<Guid>(), null));
        }

        [Fact]
        public void UpdateAsync_CallsUpdateAsyncMethod_WhenTeamIsValid()
        {
            var team = new Team { Owner = new User { Id = Guid.NewGuid() } };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Teams.UpdateAsync(It.IsAny<Team>()));
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Teams.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(team);

            _sut.UpdateAsync(team.Owner.Id, new TeamDto());

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Teams.UpdateAsync(It.IsAny<Team>()), Times.Once);
        }

        [Fact]
        public async Task UpdateAsync_ThrowsEntityNotFoundException_WhenTeamIsNotFound()
        {
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Teams.UpdateAsync(It.IsAny<Team>()));
            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Teams.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.UpdateAsync(It.IsAny<Guid>(), new TeamDto()));
        }

        [Fact]
        public async Task UpdateAsync_ThrowsNullReferenceException_WhenInputIsNull()
        {
            await Assert.ThrowsAsync<NullReferenceException>(() => _sut.UpdateAsync(It.IsAny<Guid>(), null));
        }

        [Fact]
        public async Task DeleteAsync_ThrowsException_WhenInputIsNull()
        {
            _unitOfWorkMock.Setup(unitOfWork => unitOfWork.Teams.DeleteAsync(It.IsAny<Guid>()));

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.DeleteAsync(It.IsAny<Guid>(), Guid.Empty));
        }

        [Fact]
        public void DeleteAsync_CallsDeleteMethod_WhenTeamExists()
        {
            var team = new Team { Owner = new User { Id = Guid.NewGuid() } };

            _unitOfWorkMock
                .Setup(x => x.Teams.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(team);

            _sut.DeleteAsync(team.Owner.Id, Guid.NewGuid());

            _unitOfWorkMock.Verify(unitOfWork => unitOfWork.Teams.DeleteAsync(It.IsAny<Guid>()), Times.Once);
        }

        [Fact]
        public async Task DeleteAsync_ThrowsEntityNotFoundException_WhenTeamDoesNotExist()
        {
            _unitOfWorkMock
                .Setup(x => x.Teams.GetAsync(It.IsAny<Guid>()))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.DeleteAsync(It.IsAny<Guid>(), Guid.NewGuid()));
        }

        [Fact]
        public async Task AddParticipantAsync_AddsUserToTeam_WhenTeamExists()
        {
            var team = new Team { Id = Guid.NewGuid(), Owner = new User { Id = Guid.NewGuid() } };
            var user = new UserDto { Id = Guid.NewGuid() };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Teams.GetAsync(team.Id))
                .ReturnsAsync(team);

            await _sut.AddParticipantAsync(team.Owner.Id, team.Id, user);

            _unitOfWorkMock.Verify(work => work.Teams.UpdateAsync(It.IsAny<Team>()), Times.Once);
        }

        [Fact]
        public async Task AddParticipantAsync_ThrowsEntityExistsException_WhenUserHasAlreadyParticipatedInTeam()
        {
            var userId = Guid.NewGuid();
            var team = new Team
            {
                Id = Guid.NewGuid(),
                Owner = new User { Id = Guid.NewGuid() },
                Participants = new List<User> { new User { Id = userId } }
            };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Teams.GetAsync(team.Id))
                .ReturnsAsync(team);

            await Assert.ThrowsAsync<EntityExistsException>(() => _sut.AddParticipantAsync(team.Owner.Id, team.Id, new UserDto { Id = userId }));
        }

        [Fact]
        public async Task AddParticipantAsync_ThrowsEntityNotFoundException_WhenTeamDoesNotExist()
        {
            var team = new Team { Id = Guid.NewGuid(), Owner = new User { Id = Guid.NewGuid() } };

            _unitOfWorkMock
                .Setup(unitOfWork => unitOfWork.Teams.GetAsync(team.Id))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.AddParticipantAsync(team.Owner.Id, team.Id, new UserDto()));
        }

        [Fact]
        public async Task RemoveParticipantAsync_RemovesUserFromTeam_WhenTeamExistsAndUserIsItsMember()
        {
            var team = new Team
            {
                Id = Guid.NewGuid(),
                Participants = new List<User>
                {
                    new User
                    {
                        Id = Guid.NewGuid()
                    }
                },
                Owner = new User { Id = Guid.NewGuid() }
            };
            var userId = team.Participants.FirstOrDefault().Id;

            _unitOfWorkMock
                .Setup(x => x.Teams.GetAsync(team.Id))
                .ReturnsAsync(team);

            await _sut.RemoveParticipantAsync(team.Owner.Id, team.Id, userId);

            _unitOfWorkMock.Verify(work => work.Teams.UpdateAsync(It.IsAny<Team>()), Times.Once);
        }

        [Fact]
        public async Task RemoveParticipantAsync_ThrowsServiceException_WhenUserIsNotATeamMember()
        {
            var team = new Team
            {
                Id = Guid.NewGuid(),
                Participants = new List<User>(),
                Owner = new User { Id = Guid.NewGuid() }
            };

            var userId = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(x => x.Teams.GetAsync(team.Id))
                .ReturnsAsync(team);

            await Assert.ThrowsAsync<ServiceException>(() => _sut.RemoveParticipantAsync(team.Owner.Id, team.Id, userId));
        }

        [Fact]
        public async Task RemoveParticipantAsync_ThrowsEntityNotFoundException_WhenTeamDoesNotExist()
        {
            var teamId = Guid.NewGuid();
            var userId = Guid.NewGuid();

            _unitOfWorkMock
                .Setup(x => x.Teams.GetAsync(teamId))
                .ReturnsAsync(null);

            await Assert.ThrowsAsync<EntityNotFoundException>(() => _sut.RemoveParticipantAsync(It.IsAny<Guid>(), teamId, userId));
        }
    }
}