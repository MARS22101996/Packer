using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using EpamMA.Communication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerUI.ApiModels;
using TaskManagerUI.ViewModels;
using TaskManagerUI.ViewModels.StatisticViewModels;

namespace TaskManagerUI.Controllers
{
    [Authorize]
    public class StatisticController : BaseController
    {
        private readonly ICommunicationService _communicationService;
        private readonly IMapper _mapper;

        public StatisticController(ICommunicationService communicationService, IMapper mapper)
        {
            _communicationService = communicationService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Dashboard()
        {
            const int defaultStatisticDays = 14;
            var teams = await _communicationService
                .GetAsync<IEnumerable<TeamApiModel>>("user/teams", null, FormHeaders(JsonType), "teamapi");

            if (!teams.Any())
            {
                return RedirectToAction("Board", "Ticket");
            }

            var model = new DashboardViewModel
            {
                Teams = _mapper.Map<IEnumerable<TeamViewModel>>(teams),
                StartDate = DateTime.UtcNow.AddDays(-defaultStatisticDays),
                SelectedTeamId = teams.FirstOrDefault().Id
                
            };

            return View(model);
        }

        [HttpGet]
        public async Task<JsonResult> GetStatistic(Guid teamId, DateTime startDate)
        {
            var urlParameters = new Dictionary<string, string>
            {
                { "startDate", startDate.ToString(CultureInfo.InvariantCulture) }
            };
            var statisticJson = await _communicationService
                .GetAsync($"user/teams/{teamId}/statistic", urlParameters, FormHeaders(JsonType), "statisticapi");

            return Json(statisticJson);
        }
    }
}