using AutoMapper;
using NotificationService.BLL.DTO;
using NotificationService.DAL.Entities;

namespace NotificationService.BLL.Infrastructure.Automapper
{
    public class DtoToEntityProfile : Profile
    {
        public DtoToEntityProfile()
        {
            CreateMap<UserDto, User>();
        }
    }
}