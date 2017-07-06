using AutoMapper;
using TeamService.BLL.DTO;
using TeamService.WEB.Models;

namespace TeamService.WEB.Infrastructure.Automapper
{
    public class ApiModelToDtoProfile : Profile
    {
        public ApiModelToDtoProfile()
        {
            CreateMap<UserApiModel, UserDto>();
            CreateMap<TeamApiModel, TeamDto>();
        }
    }
}