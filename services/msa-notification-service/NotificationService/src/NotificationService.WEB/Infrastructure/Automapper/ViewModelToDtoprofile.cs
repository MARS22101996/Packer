using AutoMapper;
using NotificationService.BLL.DTO;
using NotificationService.BLL.Infrastructure;
using NotificationService.WEB.Models;

namespace NotificationService.WEB.Infrastructure.Automapper
{
    public class ViewModelToDtoProfile : Profile
    {
        public ViewModelToDtoProfile()
        {
            CreateMap<TicketApiModel, TicketDto>();
            CreateMap<NotificationInfoApiModel, NotificationInfo>();
            CreateMap<UserApiModel, UserDto>();
        }
    }
}