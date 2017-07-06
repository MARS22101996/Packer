using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using TeamService.BLL.DTO;
using TeamService.BLL.Interfaces;
using TeamService.WEB.Models;
using EpamMA.Communication.Interfaces;

namespace TeamService.WEB.Controllers
{
    [Route("TeamService/api/user/teams")]
    [Authorize]
    [SwaggerResponse((int)HttpStatusCode.OK, Description = "Success")]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "Unauthorized user")]
    public class TeamController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly ICommunicationService _communicationService;
        private readonly IMapper _mapper;
        private readonly ILogger<TeamController> _logger;

        public TeamController(
            ITeamService teamService, 
            IMapper mapper, 
            ILogger<TeamController> logger, 
            ICommunicationService communicationService)
        {
            _teamService = teamService;
            _mapper = mapper;
            _logger = logger;
            _communicationService = communicationService;
        }

        /// <summary>
        /// Returns team by id
        /// </summary>
        /// <param name="id">Team id</param>
        [HttpGet("{id}")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(TeamApiModel), Description = "Team entity")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(JsonResult), Description = "Team with such id does not exist")]
        public async Task<IActionResult> GetById(Guid? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("Id parameter wasn't set. The route is teams/{id}");
            }

            var teamDto = await _teamService.GetAsync(CurrentUserId, id.Value);
            var teamApiModel = _mapper.Map<TeamApiModel>(teamDto);

            _logger.LogInformation($"Get team with id: {id.Value}");

            return Json(teamApiModel);
        }

        /// <summary>
        /// Returns all teams for current user (where he is an owner)
        /// </summary>
        /// <param name="onlyOwner">(optional) returns only teams where user is owner</param>
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(IEnumerable<TeamApiModel>), Description = "A list of team entities")]
        public IActionResult Get([FromQuery] bool onlyOwner)
        {
            var teamDtos = _teamService.GetByUser(CurrentUserId, onlyOwner);
            var teamApiModels = _mapper.Map<IEnumerable<TeamApiModel>>(teamDtos);

            _logger.LogInformation("Get all teams");

            return Json(teamApiModels);
        }

        /// <summary>
        /// Creates new team
        /// </summary>
        /// <param name="teamApiModel">Team model</param>
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(OkResult), Description = "Team successfuly created")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Invalid model")]
        public async Task<IActionResult> Post([FromBody] TeamApiModel teamApiModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            teamApiModel.Owner = await GetRemoteUserAsync(teamApiModel.Owner.Id);

            var teamDto = _mapper.Map<TeamDto>(teamApiModel);
            var id = await _teamService.CreateAsync(CurrentUserId, teamDto);

            _logger.LogInformation($"New team {teamApiModel.Name} was created");

            return Ok(id);
        }

        /// <summary>
        /// Updates new team
        /// </summary>
        /// <param name="teamApiModel">Team model</param>
        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Invalid model")]
        public async Task<IActionResult> Put([FromBody] TeamApiModel teamApiModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            teamApiModel.Owner = await GetRemoteUserAsync(teamApiModel.Owner.Id);

            var teamDto = _mapper.Map<TeamDto>(teamApiModel);
            await _teamService.UpdateAsync(CurrentUserId, teamDto);

            _logger.LogInformation($"Team {teamApiModel.Name} was updated");

            return Ok();
        }

        /// <summary>
        /// Deletes team by id 
        /// </summary>
        /// <param name="id">Team id</param>
        [HttpDelete("{id}")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(JsonResult), Description = "Team with such id does not exist")]
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (!id.HasValue)
            {
                return BadRequest("Id parameter wasn't set. The route is teams/{id}");
            }

            await _teamService.DeleteAsync(CurrentUserId, id.Value);

            _logger.LogInformation($"Team with was deleted. Id: {id.Value}");

            return Ok();
        }

        private async Task<UserApiModel> GetRemoteUserAsync(Guid userId)
        {
            var route = $"/api/users/{userId}";
            var user = await _communicationService.GetAsync<UserApiModel>(route, null, FormHeaders("application/json"));

            return user;
        }
    }
}