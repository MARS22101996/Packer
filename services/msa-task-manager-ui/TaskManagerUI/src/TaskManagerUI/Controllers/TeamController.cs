using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EpamMA.Communication.Infrastructure.Exceptions;
using EpamMA.Communication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerUI.ApiModels;
using TaskManagerUI.Infrastructure.Authorization;
using TaskManagerUI.ViewModels;
using TaskManagerUI.ViewModels.AccountViewModels;

namespace TaskManagerUI.Controllers
{
    [Authorize]
    public class TeamController : BaseController
    {
        private readonly ICommunicationService _communicationService;
        private readonly IMapper _mapper;

        public TeamController(IMapper mapper, ICommunicationService communicationService)
        {
            _mapper = mapper;
            _communicationService = communicationService;
        }

        [HttpPost]
        public async Task<IActionResult> Create(TeamViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToAction("Board", "Ticket");
            }

            var team = new TeamApiModel
            {
                Name = model.Name,
                Owner = new UserApiModel { Id = User.GetUserId() }
            };

            var id = await _communicationService.PostAsync<string, TeamApiModel>("user/teams", team, FormHeaders(JsonType), "teamapi");
            return RedirectToAction("Board", "Ticket", new { teamId = new Guid(id) });
        }

        [HttpGet]
        public async Task<IActionResult> TeamDashboard(Guid? teamId)
        {
            var urlParams = new Dictionary<string, string>
            {
                { "ownerOnly", "false" }
            };
            var teams = (await _communicationService.GetAsync<IEnumerable<TeamApiModel>>("user/teams", urlParams, FormHeaders(JsonType), "teamapi")).ToList();

            if (teams.Count == 0)
            {
                return View(new ParticipantsViewModel());
            }

            var teamViewModels = Mapper.Map<IList<TeamViewModel>>(teams);
            var team = teamViewModels.FirstOrDefault();
            if (teamId != null)
            {
                team = teamViewModels.FirstOrDefault(t => t.Id == teamId);
            }

            var model = new ParticipantsViewModel
            {
                SelectedTeam = team,
                Teams = teamViewModels
            };

            if (User.GetUserId() == team.Owner.Id)
            {
                var participants = await _communicationService.GetAsync<IEnumerable<UserApiModel>>(
                    $"user/teams/{team.Id}/participants",
                    null,
                    FormHeaders(JsonType),
                    "teamapi");

                model.Participants = _mapper.Map<IEnumerable<UserViewModel>>(participants);
            }

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AssignUserToTeam(string userEmail, Guid? teamId)
        {
            if (string.IsNullOrEmpty(userEmail) || !teamId.HasValue)
            {
                return View("BadRequest");
            }

            try
            {
                var urlParams = new Dictionary<string, string> {{"email", userEmail}};
                var user = await _communicationService.GetAsync<UserApiModel>("users", urlParams, FormHeaders(JsonType), "userapi");

                await _communicationService.PostAsync($"user/teams/{teamId}/participants", user, FormHeaders(JsonType), "teamapi");
            }
            catch (ServiceCommunicationException)
            {

            }

            return RedirectToAction("TeamDashboard", new { teamId = teamId });
        }

        [HttpGet]
        public async Task<IActionResult> LeaveTeam(Guid? teamId)
        {
            if (!teamId.HasValue)
            {
                return View("BadRequest");
            }

            await _communicationService.DeleteAsync(
                $"user/teams/{teamId}/participants/{User.GetUserId()}",
                null,
                FormHeaders(JsonType),
                "teamapi");

            return RedirectToAction("Board", "Ticket");
        }

        [HttpGet]
        public async Task<IActionResult> AllUserEmails()
        {
            var model = (await _communicationService.GetAsync<IEnumerable<UserApiModel>>("users", null, FormHeaders(JsonType), "userapi")).ToList();

            return Json(model.Where(u => u.Id != User.GetUserId()).Select(u => u.Email));
        }
    }
}