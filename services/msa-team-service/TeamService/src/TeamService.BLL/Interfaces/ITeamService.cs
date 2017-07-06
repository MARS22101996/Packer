using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TeamService.BLL.DTO;

namespace TeamService.BLL.Interfaces
{
    public interface ITeamService
    {
        IEnumerable<TeamDto> GetByUser(Guid userId, bool onlyOwner);

        Task<TeamDto> GetAsync(Guid userId, Guid teamId);

        Task<Guid> CreateAsync(Guid ownerId, TeamDto teamDto);

        Task UpdateAsync(Guid ownerId, TeamDto teamDto);

        Task DeleteAsync(Guid ownerId, Guid teamId);

        Task<IEnumerable<UserDto>> GetParticipantsAsync(Guid ownerId, Guid teamId);

        Task AddParticipantAsync(Guid ownerId, Guid teamId, UserDto userDto);

        Task RemoveParticipantAsync(Guid ownerId, Guid teamId, Guid userId);
    }
}