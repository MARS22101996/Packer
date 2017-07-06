using System.Linq;
using AutoMapper;
using TicketService.BLL.DTO;
using TicketService.DAL.Entities;

namespace TicketService.BLL.Infrastructure.Automapper
{
    public class DTOToEntityProfile : Profile
    {
        public DTOToEntityProfile()
        {
            CreateMap<TicketDto, Ticket>()
                .ForMember(
                    ticket => ticket.LinkedTicketIds,
                    expression =>
                            expression.MapFrom(obj => obj.LinkedTicketIds.Select(guid => guid).ToList()));

            CreateMap<UserDto, User>();

            CreateMap<CommentDto, Comment>();

            CreateMap<TagDto, Tag>();
        }
    }
}