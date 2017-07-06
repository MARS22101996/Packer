using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EpamMA.Communication.Interfaces;
using Microsoft.AspNetCore.Mvc;
using TaskManagerUI.ApiModels;
using TaskManagerUI.Models.Enums;
using TaskManagerUI.ViewModels;
using TaskManagerUI.ViewModels.AccountViewModels;
using TaskManagerUI.ViewModels.CommentViewModels;
using TaskManagerUI.ViewModels.TicketViewModels;

namespace TaskManagerUI.Controllers
{
    public class TicketController : BaseController
    {
        private readonly ICommunicationService _communicationService;
        private readonly IMapper _mapper;

        public TicketController(IMapper mapper, ICommunicationService communicationService)
        {
            _mapper = mapper;
            _communicationService = communicationService;
        }

        [HttpGet]
        public async Task<IActionResult> Board(Guid? teamId)
        {
            var urlParams = new Dictionary<string, string>
            {
                { "ownerOnly", "false" }
            };
            var teams = (await _communicationService.GetAsync<IEnumerable<TeamApiModel>>("user/teams", urlParams, FormHeaders(JsonType), "teamapi")).ToList();
            var teamViewModels = Mapper.Map<IList<TeamViewModel>>(teams);

            var teamBoard = new TeamBoardViewModel { Teams = teamViewModels };

            if (teamId.HasValue && teamViewModels.Any(model => model.Id == teamId.Value))
            {
                teamBoard.SelectedTeam = teamViewModels.First(model => model.Id == teamId.Value);
            }
            else
            {
                teamBoard.SelectedTeam = teamViewModels.FirstOrDefault();
            }

            if (teamBoard.SelectedTeam != null)
            {
                var ticketBoard = await GetTicketsForTeam(teamBoard.SelectedTeam.Id);

                teamBoard.SelectedTeamTickets = ticketBoard;
            }

            return View(teamBoard);
        }

        [HttpGet]
        public async Task<IActionResult> GetTickets(Guid? teamId)
        {
            var urlParams = new Dictionary<string, string>
            {
                { "ownerOnly", "false" }
            };
            var teams = (await _communicationService.GetAsync<IEnumerable<TeamApiModel>>("user/teams", urlParams, FormHeaders(JsonType), "teamapi")).ToList();
            var teamViewModels = Mapper.Map<IList<TeamViewModel>>(teams);

            var teamBoard = new TeamBoardViewModel { Teams = teamViewModels };

            if(teamId.HasValue && teamViewModels.Any(model => model.Id == teamId.Value))
            {
                teamBoard.SelectedTeam = teamViewModels.First(model => model.Id == teamId.Value);
            }
            else
            {
                teamBoard.SelectedTeam = teamViewModels.FirstOrDefault();
            }

            if(teamBoard.SelectedTeam != null)
            {
                var ticketBoard = await GetTicketsForTeam(teamBoard.SelectedTeam.Id);

                teamBoard.SelectedTeamTickets = ticketBoard;
            }

            return Json(teamBoard);
        }

        [HttpGet]
        public async Task<IActionResult> TicketDetails(Guid teamId, Guid ticketId)
        {
            var comments = await _communicationService
                .GetAsync<IEnumerable<CommentApiModel>>($"user/teams/{teamId}/tickets/{ticketId}/comments", null, FormHeaders(JsonType), "ticketapi");
            var commentViewModels = _mapper.Map<IList<CommentViewModel>>(comments);

            foreach (var comment in commentViewModels)
            {
                comment.TeamId = teamId;
                comment.TicketId = ticketId;
            }

            var ticket = await _communicationService
                .GetAsync<TicketApiModel>($"user/teams/{teamId}/tickets/{ticketId}", null, FormHeaders(JsonType), "ticketapi");
            var ticketViewModel = _mapper.Map<TicketViewModel>(ticket);
            ticketViewModel.TeamId = teamId;

            var wathcers = await _communicationService.GetAsync<IEnumerable<UserApiModel>>(
                $"user/teams/{teamId}/tickets/{ticketViewModel.Id}/watchers",
                null,
                FormHeaders(JsonType),
                "notifyapi");
            ticketViewModel.Watchers = _mapper.Map<IEnumerable<UserViewModel>>(wathcers);


            var linkedTickets = await _communicationService.GetAsync<IEnumerable<TicketApiModel>>(
                $"user/teams/{teamId}/tickets/{ticketId}/linked",
                null,
                FormHeaders(JsonType),
                "ticketapi");

            var linkedTicketsViewModel = _mapper.Map<IEnumerable<TicketViewModel>>(linkedTickets);
            ticketViewModel.LinkedTickets = linkedTicketsViewModel;

            var ticketDetails = new TicketDetailsViewModel
            {
                Comments = commentViewModels,
                Ticket = ticketViewModel
            };

            return Json(ticketDetails);
        }

        [HttpGet]
        public async Task<IActionResult> Create(Guid teamId)
        {
            var tags = await _communicationService.GetAsync<IEnumerable<TagApiModel>>("tags", null, FormHeaders(JsonType), "ticketapi");
            var team = await _communicationService.GetAsync<TeamApiModel>($"user/teams/{teamId}", null, FormHeaders(JsonType), "teamapi");
            var participants = await _communicationService.GetAsync<IEnumerable<UserApiModel>>($"user/teams/{teamId}/participants", null, FormHeaders(JsonType), "teamapi");
            participants = participants.Append(team.Owner);

            var tickets = await _communicationService.GetAsync<IEnumerable<TicketApiModel>>($"user/teams/{teamId}/tickets", null, FormHeaders(JsonType), "ticketapi");

            var model = new CreateTicketViewModel
            {
                TeamId = teamId,
                AllTags = tags.Select(tagDto => tagDto.Name),
                AllTickets = tickets,
                Tags = new List<string>(),
                Assignees = participants
            };

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody]TicketViewModel ticketViewModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var ticket = _mapper.Map<TicketApiModel>(ticketViewModel);
            ticket.LinkedTicketIds = ticketViewModel.LinkedTickets.Select(t => t.Id).ToList();
            ticket.Status = Status.Open;
            var ticketId = await _communicationService.PostAsync($"user/teams/{ticketViewModel.TeamId}/tickets", ticket, FormHeaders(JsonType), "ticketapi");

            return Json(ticketId);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid? teamId, Guid? ticketId)
        {
            if (!teamId.HasValue || !ticketId.HasValue)
            {
                return View("BadRequest");
            }

            var tags = await _communicationService.GetAsync<IEnumerable<TagApiModel>>("tags", null, FormHeaders(JsonType), "ticketapi");
            var team = await _communicationService.GetAsync<TeamApiModel>($"user/teams/{teamId}", null, FormHeaders(JsonType), "teamapi");
            var participants = await _communicationService.GetAsync<IEnumerable<UserApiModel>>($"user/teams/{teamId}/participants", null, FormHeaders(JsonType), "teamapi");
            participants = participants.Append(team.Owner);

            var tickets = (await _communicationService.GetAsync<IEnumerable<TicketApiModel>>($"user/teams/{teamId}/tickets", null, FormHeaders(JsonType), "ticketapi")).ToList();
            tickets.RemoveAt(tickets.FindIndex(t => t.Id == ticketId.Value));
            var ticket = await _communicationService
                .GetAsync<TicketApiModel>($"user/teams/{teamId}/tickets/{ticketId.Value}", null, FormHeaders(JsonType), "ticketapi");
            
            var model = _mapper.Map<CreateTicketViewModel>(ticket);
            
            model.TeamId = teamId.Value;
            model.AllTags = tags.Select(tagDto => tagDto.Name);
            model.AllTickets = tickets;
            model.Assignees = participants;
            model.LinkedTickets = await _communicationService
                .GetAsync<IEnumerable<TicketApiModel>>($"user/teams/{teamId}/tickets/{ticketId.Value}/linked", null, FormHeaders(JsonType), "ticketapi");

            return Json(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody]TicketViewModel ticketModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var ticket = _mapper.Map<TicketApiModel>(ticketModel);
            ticket.LinkedTicketIds = ticketModel.LinkedTickets.Select(t => t.Id).ToList();
            var ticketId = await _communicationService
                .PutAsync($"user/teams/{ticketModel.TeamId}/tickets", ticket, FormHeaders(JsonType), "ticketapi");

            return Json(ticketId);
        }

        [HttpGet]
        public async Task<IActionResult> EditStatus(Guid? teamId, Guid? ticketId, Status status)
        {
            if (!teamId.HasValue || !ticketId.HasValue)
            {
                return View("BadRequest");
            }

            var ticket = await _communicationService.GetAsync<TicketApiModel>($"user/teams/{teamId.Value}/tickets/{ticketId.Value}", null, FormHeaders(JsonType), "ticketapi");

            ticket.Status = status;

            await _communicationService.PutAsync($"user/teams/{teamId.Value}/tickets", ticket, FormHeaders(JsonType), "ticketapi");

            return RedirectToAction("Board", "Ticket", new { teamId = teamId.Value });
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid? teamId, Guid? ticketId)
        {
            if (!teamId.HasValue || !ticketId.HasValue)
            {
                return View("BadRequest");
            }

            await _communicationService
                .DeleteAsync($"user/teams/{teamId.Value}/tickets/{ticketId}", null, FormHeaders(JsonType), "ticketapi");

            return RedirectToAction("Board", new { teamId = teamId.Value });
        }


        [HttpGet]
        public async Task<IActionResult> Watch(Guid? teamId, Guid? ticketId, Guid? userId)
        {
            if (!teamId.HasValue || !ticketId.HasValue || !userId.HasValue)
            {
                return View("BadRequest");
            }

            var user = await _communicationService.GetAsync<UserApiModel>($"users/{userId}", null, FormHeaders(JsonType));

            await _communicationService
                .PostAsync($"user/teams/{teamId.Value}/tickets/{ticketId}/watchers", user, FormHeaders(JsonType), "notifyapi");

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Unwatch(Guid? teamId, Guid? ticketId, Guid? userId)
        {
            if (!teamId.HasValue || !ticketId.HasValue || !userId.HasValue)
            {
                return View("BadRequest");
            }

            await _communicationService
                .DeleteAsync($"user/teams/{teamId.Value}/tickets/{ticketId}/watchers/{userId}", null, FormHeaders(JsonType), "notifyapi");

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> SearchTickets(Guid? teamId, string searchString)
        {
            if (!teamId.HasValue)
            {
                return View("BadRequest");
            }

            var urlParams = new Dictionary<string, string>
            {
                { "searchString", searchString }
            };

            var filteredTickets = await _communicationService.GetAsync<IEnumerable<TicketApiModel>>($"user/teams/{teamId}/tickets/search", urlParams, FormHeaders(JsonType), "ticketapi");
            var filteredTicketViewModels = _mapper.Map<IEnumerable<TicketViewModel>>(filteredTickets);

            ViewBag.searchString = searchString;

            return View(filteredTicketViewModels);
        }

        private async Task<TicketBoardViewModel> GetTicketsForTeam(Guid teamId)
        {
            var tickets = await _communicationService
                .GetAsync<IEnumerable<TicketApiModel>>($"user/teams/{teamId}/tickets", null, FormHeaders(JsonType), "ticketapi");
            var ticketViewModels = _mapper.Map<IList<TicketViewModel>>(tickets);

            foreach (var ticket in ticketViewModels)
            {
                ticket.TeamId = teamId;
            }

            var ticketBoard = new TicketBoardViewModel
            {
                PlannedTasks = ticketViewModels.Where(ticketViewModel => ticketViewModel.Status == Status.Open),
                ProgressTasks = ticketViewModels.Where(ticketViewModel => ticketViewModel.Status == Status.InProgress),
                DoneTasks = ticketViewModels.Where(ticketViewModel => ticketViewModel.Status == Status.Done)
            };

            return ticketBoard;
        }
    }
}