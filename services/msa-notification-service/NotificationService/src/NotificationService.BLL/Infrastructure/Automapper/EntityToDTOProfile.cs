using AutoMapper;
using NotificationService.BLL.DTO;
using NotificationService.DAL.Entities;

namespace NotificationService.BLL.Infrastructure.Automapper
{
    public class EntityToDtoProfile : Profile
    {
        public EntityToDtoProfile()
        {
            CreateMap<User, UserDto>();
        }
    }
}