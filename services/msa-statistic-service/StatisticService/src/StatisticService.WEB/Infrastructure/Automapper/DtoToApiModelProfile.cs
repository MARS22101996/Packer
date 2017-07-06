using AutoMapper;
using StatisticService.BLL.DTO;
using StatisticService.WEB.Models.StatisticApiModels;

namespace StatisticService.WEB.Infrastructure.Automapper
{
    public class DtoToApiModelProfile : Profile
    {
        public DtoToApiModelProfile()
        {
            CreateMap<TicketDto, TicketApiModel>();
            CreateMap<UserDto, UserApiModel>();
            CreateMap<TicketDto, TicketApiModel>();
        }
    }
}