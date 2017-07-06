using AutoMapper;
using TicketService.BLL.DTO;
using TicketService.WEB.Models;

namespace TicketService.WEB.Infrastructure.AutoMapper
{
    public class ApiModelToDtoProfile : Profile
    {
        public ApiModelToDtoProfile()
        {
            CreateMap<TicketApiModel, TicketDto>();

            CreateMap<UserApiModel, UserDto>();

            CreateMap<TicketApiModel, TicketDto>();

            CreateMap<TagApiModel, TagDto>();

            CreateMap<CommentApiModel, CommentDto>();

            CreateMap<FilterApiModel, FilterDto>();
        }
    }
}