using AutoMapper;
using NotificationService.BLL.DTO;
using NotificationService.BLL.Infrastructure;
using NotificationService.WEB.Models;

namespace NotificationService.WEB.Infrastructure.Automapper
{
    public class DtoToViewModelProfile : Profile
    {
        public DtoToViewModelProfile()
        {
            CreateMap<TicketDto, TicketApiModel>();
            CreateMap<NotificationInfo, NotificationInfoApiModel>();
            CreateMap<UserDto, UserApiModel>();
        }
    }
}