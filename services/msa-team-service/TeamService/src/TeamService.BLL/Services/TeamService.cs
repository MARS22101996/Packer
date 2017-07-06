using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using TeamService.BLL.DTO;
using TeamService.BLL.Infrastructure.Exceptions;
using TeamService.BLL.Interfaces;
using TeamService.DAL.Entities;
using TeamService.DAL.Interfaces;

namespace TeamService.BLL.Services
{
    public class TeamService : ITeamService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public TeamService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public IEnumerable<TeamDto> GetByUser(Guid userId, bool onlyOwner)
        {
            Func<Team, bool> userIsParticipant = team => 
                !onlyOwner && team.Participants.Select(user => user.Id).Contains(userId);
            Func<Team, bool> userIsOwner = team => team.Owner.Id == userId;

            var teams = _unitOfWork.Teams
                .Find(team => userIsOwner.Invoke(team) || userIsParticipant.Invoke(team));
            var teamDtos = _mapper.Map<IEnumerable<TeamDto>>(teams);

            return teamDtos;
        }

        public async Task<TeamDto> GetAsync(Guid userId, Guid teamId)
        {
            var team = await _unitOfWork.Teams.GetAsync(teamId);

            if (team == null)
            {
                throw new EntityNotFoundException($"Team with such id doesn't exist. Id: {teamId}", "Team");
            }

            Func<Team, bool> userIsParticipant = t =>
                t.Participants.Select(user => user.Id).Contains(userId);
            Func<Team, bool> userIsOwner = t => t.Owner.Id == userId;

            if (!userIsOwner.Invoke(team) && !userIsParticipant.Invoke(team))
            {
                throw new EntityNotFoundException(
                    $"User is not an owner or participant of requested team. TeamId: {teamId}. UserId: {userId}", "Team");
            }

            var teamDto = _mapper.Map<TeamDto>(team);

            return teamDto;
        }

        public async Task<Guid> CreateAsync(Guid ownerId, TeamDto teamDto)
        {
            var team = _mapper.Map<Team>(teamDto);
            ValidateOwner(ownerId, team);

            return await _unitOfWork.Teams.CreateAsync(team);
        }

        public async Task UpdateAsync(Guid ownerId, TeamDto teamDto)
        {
            var originTeam = await _unitOfWork.Teams.GetAsync(teamDto.Id);

            if (originTeam == null)
            {
                throw new EntityNotFoundException($"Team with such id doesn't exist. Id: {teamDto.Id}", "Team");
            }

            var participants = new List<User>(originTeam.Participants);
            ValidateOwner(ownerId, originTeam);

            var updatingTeam = _mapper.Map<TeamDto, Team>(
                teamDto, 
                options => options.AfterMap((dto, team) => team.Participants = participants));

            await _unitOfWork.Teams.UpdateAsync(updatingTeam);
        }

        public async Task DeleteAsync(Guid ownerId, Guid id)
        {
            var team = await _unitOfWork.Teams.GetAsync(id);

            if (team == null)
            {
                throw new EntityNotFoundException($"Team with such id doesn't exist. Id: {id}", "Team");
            }

            ValidateOwner(ownerId, team);

            await _unitOfWork.Teams.DeleteAsync(id);
        }

        public async Task<IEnumerable<UserDto>> GetParticipantsAsync(Guid ownerId, Guid teamId)
        {
            var team = await _unitOfWork.Teams.GetAsync(teamId);

            if (team == null)
            {
                throw new EntityNotFoundException($"Team with such id doesn't exist. Id: {teamId}", "Team");
            }

            ValidateOwner(ownerId, team);

            var teamDto = _mapper.Map<TeamDto>(team);

            return teamDto.Participants;
        }

        public async Task AddParticipantAsync(Guid ownerId, Guid teamId, UserDto userDto)
        {
            var team = await _unitOfWork.Teams.GetAsync(teamId);

            if (team == null)
            {
                throw new EntityNotFoundException($"Team with such id doesn't exist. Id: {teamId}", "Team");
            }

            ValidateOwner(ownerId, team);

            if (team.Participants.All(x => x.Id != userDto.Id))
            {
                var user = _mapper.Map<User>(userDto);

                team.Participants = team.Participants.Append(user);

                await _unitOfWork.Teams.UpdateAsync(team);
            }
            else
            {
                throw new EntityExistsException($"User {userDto.Id} is already in a team {teamId}.", "Team");
            }
        }

        public async Task RemoveParticipantAsync(Guid ownerId, Guid teamId, Guid userId)
        {
            var team = await _unitOfWork.Teams.GetAsync(teamId);

            if (team == null)
            {
                throw new EntityNotFoundException($"Team with such id doesn't exist. Id: {teamId}", "Team");
            }

            ValidateOwner(ownerId, team);

            if (team.Participants.All(x => x.Id != userId))
            {
                throw new ServiceException(
                    $"Current user is not a member of the team. User id: {userId}. Team id: {team.Id}",
                    "User");
            }

            team.Participants = team.Participants.Where(x => x.Id != userId);

            await _unitOfWork.Teams.UpdateAsync(team);
        }

        private void ValidateOwner(Guid userId, Team teamDto)
        {
            if (teamDto.Owner.Id != userId)
            {
                throw new ServiceException(
                    $"Current user is not an owner of the team. User id: {userId}. Team id: {teamDto.Id}",
                    "User");
            }
        }
    }
}