using AutoMapper;
using TicketService.BLL.DTO;
using TicketService.WEB.Models;

namespace TicketService.WEB.Infrastructure.AutoMapper
{
    public class DtoToApiModelProfile : Profile
    {
        public DtoToApiModelProfile()
        {
            CreateMap<TicketDto, TicketApiModel>();

            CreateMap<UserDto, UserApiModel>();

            CreateMap<TagDto, TagApiModel>();

            CreateMap<CommentDto, CommentApiModel>();

            CreateMap<FilterDto, FilterApiModel>();
        }
    }
}