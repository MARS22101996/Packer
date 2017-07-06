using AutoMapper;
using TaskManagerUI.ApiModels;
using TaskManagerUI.ViewModels;
using TaskManagerUI.ViewModels.AccountViewModels;
using TaskManagerUI.ViewModels.CommentViewModels;
using TaskManagerUI.ViewModels.TicketViewModels;

namespace TaskManagerUI.Infrastructure.AutoMapper
{
    public class ViewModelToApiModelProfile : Profile
    {
        public ViewModelToApiModelProfile()
        {
            CreateMap<TicketViewModel, TicketApiModel>().ForMember(ticket => ticket.LinkedTicketIds, expression => expression.Ignore());
            CreateMap<CommentViewModel, CommentApiModel>();
            CreateMap<TeamViewModel, TeamApiModel>();
            CreateMap<UserViewModel, UserApiModel>();
        }
    }
}