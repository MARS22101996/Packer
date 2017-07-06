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
    [Route("TeamService/api/user/teams/{teamId}/participants")]
    [Authorize]
    [SwaggerResponse((int)HttpStatusCode.OK, Description = "Success")]
    [SwaggerResponse((int)HttpStatusCode.Unauthorized, Description = "Unauthorized user")]
    public class ParticipantController : BaseController
    {
        private readonly ITeamService _teamService;
        private readonly ICommunicationService _communicationService;
        private readonly IMapper _mapper;
        private readonly ILogger<TeamController> _logger;

        public ParticipantController(
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
        /// Returns team participants
        /// </summary>
        /// <param name="teamId">Team id</param>
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(IEnumerable<TeamApiModel>), Description = "A list of team participants (users)")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(JsonResult), Description = "TeamId parameter wasn't set")]
        public async Task<IActionResult> Get(Guid? teamId)
        {
            if (!teamId.HasValue)
            {
                return BadRequest("TeamId parameter wasn't set. The route is teams/{id}");
            }

            var participantDtos = await _teamService.GetParticipantsAsync(CurrentUserId, teamId.Value);
            var participantApis = _mapper.Map<IEnumerable<UserApiModel>>(participantDtos);

            _logger.LogInformation($"Get all participants of team with id: {teamId}");

            return Json(participantApis);
        }

        /// <summary>
        /// Add new participant to team
        /// </summary>
        /// <param name="teamId">Team id</param>
        /// <param name="userApiModel">Participant</param>
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(IEnumerable<TeamApiModel>), Description = "Participant successfuly added to team")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(JsonResult), Description = "Model is invalid or team id isn't set")]
        public async Task<IActionResult> Post(Guid? teamId, [FromBody] UserApiModel userApiModel)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!teamId.HasValue)
            {
                return BadRequest("TeamId parameter wasn't set. The route is teams/{id}");
            }

            var user = await GetRemoteUserAsync(userApiModel.Id);
            var userDto = _mapper.Map<UserDto>(user);

            await _teamService.AddParticipantAsync(CurrentUserId, teamId.Value, userDto);

            _logger.LogInformation(
                $"New participant is added to team with id: {teamId}. Participant id: {userDto?.Id}");

            return Ok();
        }

        /// <summary>
        /// Unassign user from team
        /// </summary>
        /// <param name="teamId">Team id</param>
        /// <param name="id">Participant(user) id</param>
        [HttpDelete("{id}")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(JsonResult), Description = "One of the method`s parametrs is null")]
        public async Task<IActionResult> Delete(Guid? teamId, Guid? id)
        {
            if (!teamId.HasValue || !id.HasValue)
            {
                return BadRequest("Team or user id parameter was not set.");
            }

            await _teamService.RemoveParticipantAsync(CurrentUserId, teamId.Value, id.Value);

            _logger.LogInformation($"User with Id: {id} was unassigned to team with Id: {teamId}");

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