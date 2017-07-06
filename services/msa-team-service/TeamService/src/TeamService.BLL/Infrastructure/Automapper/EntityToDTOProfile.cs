using AutoMapper;
using TeamService.BLL.DTO;
using TeamService.DAL.Entities;

namespace TeamService.BLL.Infrastructure.Automapper
{
    public class EntityToDtoProfile : Profile
    {
        public EntityToDtoProfile()
        {
            CreateMap<User, UserDto>();
            CreateMap<Team, TeamDto>();
        }
    }
}