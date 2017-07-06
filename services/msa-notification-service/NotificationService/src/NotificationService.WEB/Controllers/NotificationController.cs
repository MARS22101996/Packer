using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NotificationService.BLL.DTO;
using NotificationService.BLL.Infrastructure;
using NotificationService.BLL.Interfaces;
using NotificationService.WEB.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace NotificationService.WEB.Controllers
{
    [Route("NotificationService/api/user/teams/{teamId}/tickets/{ticketId}")]
    [Authorize]
    [SwaggerResponse((int)HttpStatusCode.OK, Description = "Success")]
    [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal server exception")]
    public class NotificationController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationController> _logger;

        public NotificationController(INotificationService notificationService, IMapper mapper, ILogger<NotificationController> logger)
        {
            _notificationService = notificationService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of ticket watchers
        /// </summary>
        /// <param name="teamId">Team id</param>
        /// <param name="ticketId">Ticket id</param>
        [HttpGet]
        [Route("watchers")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(JsonResult), "Ticket with specified id wasn't found")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(JsonResult), "Some arguments weren't set")]
        public async Task<IActionResult> Get(Guid? teamId, Guid? ticketId)
        {
            if (!teamId.HasValue || !ticketId.HasValue)
            {
                return BadRequest("Some arguments weren't set");
            }

            var watcherDtos = await _notificationService.GetAllWatchersAsync(teamId.Value, ticketId.Value);
            var watcherApiModels = _mapper.Map<IEnumerable<UserApiModel>>(watcherDtos);

            _logger.LogInformation($"Get watcher for ticket with id: {ticketId}");

            return Ok(watcherApiModels);
        }

        /// <summary>
        /// Returns watcher of ticket
        /// </summary>
        /// <param name="teamId">Team id</param>
        /// <param name="ticketId">Ticket id</param>
        /// <param name="watcherId">Watcher id</param>
        [HttpGet("watchers/{watcherId}")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(UserApiModel), "Ticket watcher")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(JsonResult), "User doesn't watch for this ticket")]
        public async Task<IActionResult> Get(Guid? teamId, Guid? ticketId, Guid? watcherId)
        {
            if (!teamId.HasValue || !ticketId.HasValue || !watcherId.HasValue)
            {
                return BadRequest("Some arguments weren't set");
            }

            var watcherDto = await _notificationService.GetWatcherAsync(teamId.Value, ticketId.Value, watcherId.Value);
            var watcherApiModel = _mapper.Map<UserApiModel>(watcherDto);

            _logger.LogInformation($"Get watcher with id: {watcherId} for ticket with id: {ticketId}");

            return Ok(watcherApiModel);
        }

        /// <summary>
        /// Adds new watcher to ticket
        /// </summary>
        /// <param name="teamId">Team id</param>
        /// <param name="ticketId">Ticket id</param>
        /// <param name="user">User(new watcher)</param>
        [HttpPost("watchers")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(JsonResult), "Model is not valid")]
        public async Task<IActionResult> Post(Guid? teamId, Guid? ticketId, [FromBody] UserApiModel user)
        {
            if (!teamId.HasValue || !ticketId.HasValue)
            {
                return BadRequest("Some arguments weren't set");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userDto = _mapper.Map<UserDto>(user);
            await _notificationService.AddWatcherAsync(ticketId.Value, userDto);

            _logger.LogInformation($"Add new watcher with id: {user.Id} to ticket with id: {ticketId}");

            return Ok();
        }

        /// <summary>
        /// Notifies ticket watchers
        /// </summary>
        /// <param name="teamId">Team id</param>
        /// <param name="ticketId">Ticket id</param>
        /// <param name="notificationInfo">Notification info</param>
        [HttpPost("notify")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(JsonResult), "Model is not valid")]
        public async Task<IActionResult> Notify(Guid? teamId, Guid? ticketId, [FromBody] NotificationInfoApiModel notificationInfo)
        {
            if (!teamId.HasValue || !ticketId.HasValue)
            {
                return BadRequest("Some arguments weren't set");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var info = _mapper.Map<NotificationInfo>(notificationInfo);
            await _notificationService.NotifyTicketWatchersAsync(teamId.Value, ticketId.Value, info);

            _logger.LogInformation($"Notify watchers for ticket with id: {ticketId}");

            return Ok();
        }

        /// <summary>
        /// Removes watcher from ticket
        /// </summary>
        /// <param name="teamId">Team id</param>
        /// <param name="ticketId">Ticket id</param>
        /// <param name="watcherId">Watcher id</param>
        [HttpDelete("watchers/{watcherId}")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(JsonResult), "User doesn't watch for this ticket")]
        public async Task<IActionResult> Delete(Guid? teamId, Guid? ticketId, Guid? watcherId)
        {
            if (!teamId.HasValue || !ticketId.HasValue || !watcherId.HasValue)
            {
                return BadRequest("Some arguments weren't set");
            }

            await _notificationService.RemoveWatcherAsync(teamId.Value, ticketId.Value, watcherId.Value);

            _logger.LogInformation($"Remove watcher with id: {watcherId} from ticket with id: {ticketId}");

            return Ok();
        }
    }
}