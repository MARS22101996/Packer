using AutoMapper;
using StatisticService.BLL.DTO;
using StatisticService.WEB.Models.StatisticApiModels;

namespace StatisticService.WEB.Infrastructure.Automapper
{
    public class ApiModelToDtoProfile : Profile
    {
        public ApiModelToDtoProfile()
        {
            CreateMap<UserApiModel, UserDto>(); 
            CreateMap<TicketApiModel, TicketDto>();
        }
    }
}