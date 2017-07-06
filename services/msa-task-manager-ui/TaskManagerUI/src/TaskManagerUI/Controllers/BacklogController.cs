using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EpamMA.Communication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TaskManagerUI.ApiModels;
using TaskManagerUI.ViewModels;
using TaskManagerUI.ViewModels.BacklogViewModels;
using TaskManagerUI.ViewModels.TicketViewModels;

namespace TaskManagerUI.Controllers
{
    public class BacklogController : BaseController
    {
        private readonly ICommunicationService _communicationService;
        private readonly IMapper _mapper;

        public BacklogController(IMapper mapper, ICommunicationService communicationService)
        {
            _mapper = mapper;
            _communicationService = communicationService;
        }

        [HttpGet]
        public async Task<IActionResult> List(BackLogViewModel model, Guid? teamId)
        {
            var urlParams = new Dictionary<string, string>
            {
                { "ownerOnly", "false" }
            };
            var teams = (await _communicationService.GetAsync<IEnumerable<TeamApiModel>>("user/teams", urlParams, FormHeaders(JsonType), "teamapi")).ToList();
            var teamViewModels = Mapper.Map<IList<TeamViewModel>>(teams);

            var teamBoard = new TeamBoardViewModel { Teams = teamViewModels };

            if (teamId.HasValue && teamViewModels.Any(m => m.Id == teamId.Value))
            {
                teamBoard.SelectedTeam = teamViewModels.First(m => m.Id == teamId.Value);
            }
            else
            {
                teamBoard.SelectedTeam = teamViewModels.FirstOrDefault();
            }

            if (teamBoard.SelectedTeam != null)
            {
                var tickets = await GetTicketsForTeam(teamBoard.SelectedTeam.Id);
                foreach (var ticketViewModel in tickets)
                {
                    ticketViewModel.TeamId = teamBoard.SelectedTeam.Id;
                }
                var countTickets = tickets.Count;
                var pageViewModel = new PageViewModel(countTickets, model.Page, model.PageSize);

                var backlogViewModel = new BackLogViewModel
                {
                    SelectedTeam = teamViewModels.FirstOrDefault(),
                    Teams = teamViewModels,
                    Tickets = tickets,
                    SelectedStatuses = model.SelectedStatuses,
                    SelectedPriorities = model.SelectedPriorities,
                    PageViewModel = pageViewModel,
                    TeamId = model.TeamId
                };

                return View(backlogViewModel);
            }

            return View(new BackLogViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets([FromQuery] BackLogViewModel model, Guid? teamId)
        {
            var urlParams = new Dictionary<string, string>
            {
                {"ownerOnly", "false"}
            };
            var teams =
            (await _communicationService.GetAsync<IEnumerable<TeamApiModel>>("user/teams", urlParams,
                FormHeaders(JsonType), "teamapi")).ToList();
            var teamViewModels = Mapper.Map<IList<TeamViewModel>>(teams);

            var teamBoard = new TeamBoardViewModel { Teams = teamViewModels };

            if (teamId.HasValue && teamViewModels.Any(m => m.Id == teamId.Value))
            {
                teamBoard.SelectedTeam = teamViewModels.First(m => m.Id == teamId.Value);
            }
            else
            {
                teamBoard.SelectedTeam = teamViewModels.FirstOrDefault();
            }

            var tickets = await GetTicketsForTeam(teamBoard.SelectedTeam.Id);
            foreach (var ticketViewModel in tickets)
            {
                ticketViewModel.TeamId = teamBoard.SelectedTeam.Id;
            }
            var countTickets = tickets.Count;
            var pageViewModel = new PageViewModel(countTickets, model.Page, model.PageSize);

            var backlogViewModel = new BackLogViewModel
            {
                SelectedTeam = teamViewModels.FirstOrDefault(),
                Teams = teamViewModels,
                Tickets = tickets,
                SelectedStatuses = model.SelectedStatuses,
                SelectedPriorities = model.SelectedPriorities,
                PageViewModel = pageViewModel,
                TeamId = model.TeamId
            };

            return Json(backlogViewModel);
        }

        private async Task<List<TicketViewModel>> GetTicketsForTeam(Guid teamId)
        {
            var tickets = await _communicationService
                .GetAsync<IEnumerable<TicketApiModel>>(
                    $"user/teams/{teamId}/tickets{Request.QueryString}",
                    null,
                    FormHeaders(JsonType),
                    "ticketapi");
            var ticketViewModels = _mapper.Map<List<TicketViewModel>>(tickets);

            return ticketViewModels;
        }
    }
}