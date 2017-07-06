using AutoMapper;
using TeamService.BLL.DTO;
using TeamService.WEB.Models;

namespace TeamService.WEB.Infrastructure.Automapper
{
    public class DtoToApiModelProfile : Profile
    {
        public DtoToApiModelProfile()
        {
            CreateMap<UserDto, UserApiModel>();
            CreateMap<TeamDto, TeamApiModel>();
        }
    }
}