using AutoMapper;
using TaskManagerUI.ApiModels;
using TaskManagerUI.ViewModels;
using TaskManagerUI.ViewModels.AccountViewModels;
using TaskManagerUI.ViewModels.CommentViewModels;
using TaskManagerUI.ViewModels.TicketViewModels;

namespace TaskManagerUI.Infrastructure.AutoMapper
{
    public class ApiModelToViewModelProfile : Profile
    {
        public ApiModelToViewModelProfile()
        {
            CreateMap<TicketApiModel, TicketViewModel>();
            CreateMap<UserApiModel, UserViewModel>();
            CreateMap<CommentApiModel, CommentViewModel>();
            CreateMap<TeamApiModel, TeamViewModel>();
            CreateMap<TicketApiModel, CreateTicketViewModel>();
        }
    }
}