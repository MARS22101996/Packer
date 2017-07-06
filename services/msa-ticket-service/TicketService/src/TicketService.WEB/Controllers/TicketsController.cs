using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using EpamMA.Communication.Infrastructure.Exceptions;
using EpamMA.Communication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using TicketService.BLL.DTO;
using TicketService.BLL.Interfaces;
using TicketService.Core.Enums;
using TicketService.WEB.Models;
using Microsoft.AspNetCore.Cors;

namespace TicketService.WEB.Controllers
{
    [EnableCors("Cors")]
    [Route("TicketService/api")]
    [Authorize]
    [SwaggerResponse((int)HttpStatusCode.OK, Description = "Success")]
    public class TicketsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ITicketService _ticketService;
        private readonly ICommunicationService _communicationService;
        private readonly ILogger<TicketsController> _logger;

        public TicketsController(
            IMapper mapper,
            ITicketService ticketService,
            ICommunicationService communicationService, 
            ILogger<TicketsController> logger)
        {
            _mapper = mapper;
            _ticketService = ticketService;
            _communicationService = communicationService;
            _logger = logger;
        }

        /// <summary>
        /// Returns a list of filtered tickets
        /// </summary>
        /// <param name="teamId">team id</param>
        /// <param name="model">Filter model</param>
        [Route("user/teams/{teamId}/tickets")]
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(IEnumerable<TicketApiModel>), Description = "A list of tickets")]
        public async Task<IActionResult> Get(Guid teamId, [FromQuery] FilterApiModel model)
        {
            var ticketDtos = await _ticketService.GetAllFilteredAsync(teamId, _mapper.Map<FilterDto>(model));
            var ticketApiModels = _mapper.Map<IEnumerable<TicketApiModel>>(ticketDtos);

            _logger.LogInformation("Filtered tickets were successfully received");

            return Json(ticketApiModels);
        }

        /// <summary>
        /// Returns ticket with id
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="id">Ticket id</param>
        [Route("user/teams/{teamId}/tickets/{id}")]
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(HandleErrorInfo), Description = "Ticket with such id does not exist")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(TicketApiModel), Description = "Ticket")]
        public async Task<IActionResult> GetTicket(Guid teamId, Guid id)
        {
            var ticketDto = await _ticketService.GetAsync(teamId, id);
            var ticketApiModel = _mapper.Map<TicketApiModel>(ticketDto);

            _logger.LogInformation($"Ticket was successfully received by id: {id}");

            return Json(ticketApiModel);
        }

        /// <summary>
        /// Creates new ticket
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="ticketApiModel">Ticket model</param>
        [Route("user/teams/{teamId}/tickets")]
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Invalid Model")]
        public async Task<IActionResult> Create(Guid teamId, [FromBody]TicketApiModel ticketApiModel)
        {
            if (ModelState.IsValid)
            {
                var ticketDto = _mapper.Map<TicketDto>(ticketApiModel);
                var createdTicketId = await _ticketService.CreateAsync(teamId, ticketDto);

                _logger.LogInformation($"Ticket with name {ticketApiModel.Name} was successfully created");
                return Ok(createdTicketId);
            }

            return BadRequest(ModelState);
        }

        /// <summary>
        /// Updates ticket
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="ticketApiModel">Ticket model</param>
        [Route("user/teams/{teamId}/tickets")]
        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(HandleErrorInfo), Description = "Ticket with such id does not exist")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(HandleErrorInfo), Description = "Service communication exception")]
        public async Task<IActionResult> Update(Guid teamId, [FromBody]TicketApiModel ticketApiModel)
        {
            var oldTicketDto = await _ticketService.GetAsync(teamId, ticketApiModel.Id);
            var oldTicketApiModel = _mapper.Map<TicketApiModel>(oldTicketDto);

            ticketApiModel.CreationDate = oldTicketApiModel.CreationDate;

            var ticketDto = _mapper.Map<TicketDto>(ticketApiModel);

            await _ticketService.UpdateAsync(teamId, ticketDto);

            var notificationApiModel = new NotificationInfoApiModel
            {
                OldTicket = oldTicketApiModel,
                NewTicket = ticketApiModel,
                NotificationType = NotificationType.TicketUpdated
            };

            try
            {
                await _communicationService.PostAsync<string, NotificationInfoApiModel>(
                    $"api/user/teams/{teamId}/tickets/{ticketApiModel.Id}/notify",
                    notificationApiModel,
                    FormHeaders("application/json"),
                    "NotificationService");
            }
            catch (ServiceCommunicationException)
            {
                _logger.LogError($"Ticket update notification with id: {ticketDto.Id} was not sent!");
            }

            _logger.LogInformation($"Ticket with id {ticketApiModel.Id} was successfully updated ");

            return Ok();
        }

        /// <summary>
        /// Deletes ticket with id
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="id">Ticket id</param>
        [Route("user/teams/{teamId}/tickets/{id}")]
        [HttpDelete]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(HandleErrorInfo), Description = "Ticket with such id does not exist")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(HandleErrorInfo), Description = "Service communication exception or id does not have value")]
        public async Task<IActionResult> Delete(Guid teamId, Guid? id)
        {
            if (!id.HasValue)
            {
                return BadRequest();
            }

            var ticketDto = await _ticketService.GetAsync(teamId, id.Value);
            var ticketApiModel = _mapper.Map<TicketApiModel>(ticketDto);

            await _ticketService.DeleteAsync(teamId, ticketDto.Id);

            _logger.LogInformation($"Ticket with id {id} was successfully deleted");

            var notificationApiModel = new NotificationInfoApiModel
            {
                OldTicket = ticketApiModel,
                NewTicket = null,
                NotificationType = NotificationType.TicketDeleted
            };

            try
            {
                await _communicationService.PostAsync<IActionResult, NotificationInfoApiModel>(
                    $"api/user/teams/{teamId}/tickets/{ticketApiModel.Id}/notify",
                    notificationApiModel,
                    FormHeaders("application/json"),
                    "NotificationService");
            }
            catch (ServiceCommunicationException)
            {
                _logger.LogError($"Ticket delete notification with id: {ticketDto.Id} was not sent!");
            }

            return Ok();
        }

        /// <summary>
        /// Assigns ticket for user
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="ticketId">Ticket id</param>
        /// <param name="userId">User id</param>
        [Route("user/teams/{teamId}/tickets/{ticketId}/assignee/{userId}")]
        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(HandleErrorInfo), Description = "Service communication exception or ticket, user id does not have value")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(HandleErrorInfo), Description = "Ticket with such id does not exist")]
        public async Task<IActionResult> Assign(Guid teamId, Guid? ticketId, Guid? userId)
        {
            if (!ticketId.HasValue || !userId.HasValue)
            {
                return BadRequest();
            }

            var ticketDto = await _ticketService.GetAsync(teamId, ticketId.Value);

            var user = await _communicationService.GetAsync<UserApiModel>(
                $"api/users/{userId.Value}", null, FormHeaders("application/json"), "UserService");

            ticketDto.Assignee = _mapper.Map<UserDto>(user);
           
            await _ticketService.UpdateAsync(teamId, ticketDto);

            _logger.LogInformation($"Ticket {ticketId} was assigned to user {userId}");

            var newTicketDto = await _ticketService.GetAsync(teamId, ticketId.Value);
            var newTicketApiModel = _mapper.Map<TicketApiModel>(newTicketDto);

            var notificationApiModel = new NotificationInfoApiModel
            {
                OldTicket = _mapper.Map<TicketApiModel>(ticketDto),
                NewTicket = newTicketApiModel,
                NotificationType = NotificationType.AssigneeChanged
            };

            try
            {
                await _communicationService.PostAsync<string, NotificationInfoApiModel>(
                                    $"api/user/teams/{teamId}/tickets/{ticketDto.Id}/notify",
                                    notificationApiModel,
                                    FormHeaders("application/json"),
                                    "NotificationService");
            }
            catch (ServiceCommunicationException)
            {
                _logger.LogError($"Ticket assign notification with id: {ticketDto.Id} was not sent. User id: {user.Id}!");
            }

            return Ok();
        }

        /// <summary>
        /// Unassigns ticket from user
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="ticketId">Ticket id</param>
        [Route("user/teams/{teamId}/tickets/{ticketId}/assignee")]
        [HttpDelete]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(HandleErrorInfo), Description = "Ticket with such id does not exist")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(HandleErrorInfo), Description = "Service communication exception")]
        public async Task<IActionResult> Unassign(Guid teamId, Guid ticketId)
        {
            var ticketDto = await _ticketService.GetAsync(teamId, ticketId);
            var oldTicketApiModel = _mapper.Map<TicketApiModel>(ticketDto); 

            ticketDto.Assignee = new UserDto();
            var newTicketApiModel = _mapper.Map<TicketApiModel>(ticketDto);

            await _ticketService.UpdateAsync(teamId, ticketDto);

            _logger.LogInformation($"Ticket {ticketId} was unassign");

            var notificationApiModel = new NotificationInfoApiModel
            {
                OldTicket = oldTicketApiModel,
                NewTicket = newTicketApiModel,
                NotificationType = NotificationType.AssigneeChanged
            };

            try
            {
                await _communicationService.PostAsync<string, NotificationInfoApiModel>(
                    $"api/user/teams/{teamId}/tickets/{ticketDto.Id}/notify",
                    notificationApiModel,
                    FormHeaders("application/json"),
                    "NotificationService");
            }
            catch (ServiceCommunicationException)
            {
                _logger.LogError($"Ticket unassign notification with id: {ticketDto.Id} was not sent");
            }

            return Ok();
        }
    }
}