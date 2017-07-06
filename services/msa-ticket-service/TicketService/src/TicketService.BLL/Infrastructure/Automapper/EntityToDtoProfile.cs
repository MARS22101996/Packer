using System.Linq;
using AutoMapper;
using TicketService.BLL.DTO;
using TicketService.DAL.Entities;

namespace TicketService.BLL.Infrastructure.Automapper
{
    public class EntityToDTOProfile : Profile
    {
        public EntityToDTOProfile()
        {
            CreateMap<Ticket, TicketDto>()
                .ForMember(dto => dto.Id, expression => expression.MapFrom(ticket => ticket.Id))
                .ForMember(
                    dto => dto.LinkedTicketIds,
                    expression => expression.MapFrom(obj => obj.LinkedTicketIds.Any() ? obj.LinkedTicketIds.Select(guid => guid).ToList() : null))
                .ForMember(dto => dto.CommentCount, expression => expression.MapFrom(ticket => ticket.Comments.Count()));

            CreateMap<Tag, TagDto>();

            CreateMap<User, UserDto>();

            CreateMap<Comment, CommentDto>();
        }
    }
}