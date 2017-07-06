using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StatisticService.BLL.DTO;
using StatisticService.BLL.Interfaces;
using StatisticService.WEB.Models;
using StatisticService.WEB.Models.StatisticApiModels;
using Swashbuckle.AspNetCore.SwaggerGen;
using EpamMA.Communication.Interfaces;

namespace StatisticService.WEB.Controllers
{
    [Authorize]
    [Route("StatisticService/api/")]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal server exception")]
    [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(HandleErrorInfo), Description = "Service communication exception")]
    public class StatisticController : Controller
    {
        private readonly IStatisticService _statisticService;
        private readonly IMapper _mapper;
        private readonly ICommunicationService _communicationService;
        private readonly ILogger<StatisticController> _logger;

        public StatisticController(
            IStatisticService statisticService,
            IMapper mapper,
            ICommunicationService communicationService,
            ILogger<StatisticController> logger)
        {
            _statisticService = statisticService;
            _mapper = mapper;
            _communicationService = communicationService;
            _logger = logger;
        }

        /// <summary>
        /// Returns team statistic from determined date to now
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="startDate">Date from in format 2017-03-10</param>
        [HttpGet]
        [Route("user/teams/{teamId}/statistic/{startDate}")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(UserStatisticDto), Description = "Statistic for user")]
        public async Task<IActionResult> GetStatistic(Guid teamId, DateTime startDate)
        {
            var tickets = await _communicationService.GetAsync<IEnumerable<TicketApiModel>>($"api/user/teams/{teamId}/tickets", null, Request.Headers);

            _logger.LogInformation("Team data were successfully received from communication service");

            var ticketsDto = _mapper.Map<List<TicketDto>>(tickets);
            var statistic = _statisticService.GetStatisticFiltered(startDate, ticketsDto);
            var jsonSettings = new JsonSerializerSettings
            {
                DateFormatString = "dd/MM/yyy",
                Formatting = Formatting.Indented,
                Converters = new List<JsonConverter> { new StringEnumConverter() }
            };

            var modelJson = JsonConvert.SerializeObject(statistic, jsonSettings);

            return Json(modelJson);
        }
    }
}