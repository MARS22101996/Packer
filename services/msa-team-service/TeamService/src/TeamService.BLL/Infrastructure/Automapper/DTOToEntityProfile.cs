using AutoMapper;
using TeamService.BLL.DTO;
using TeamService.DAL.Entities;

namespace TeamService.BLL.Infrastructure.Automapper
{
    public class DtoToEntityProfile : Profile
    {
        public DtoToEntityProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<TeamDto, Team>();
        }
    }
}