using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using TicketService.BLL.Interfaces;
using TicketService.WEB.Models;

namespace TicketService.WEB.Controllers
{
    [Authorize]
    [Route("TicketService/api")]
    public class LinkedTicketsController : Controller
    {
        private readonly ITicketLinkService _ticketLinkService;
        private readonly ILogger<LinkedTicketsController> _logger;
        private readonly IMapper _mapper;

        public LinkedTicketsController(
            ITicketLinkService ticketLinkService,
            ILogger<LinkedTicketsController> logger,
            IMapper mapper)
        {
            _mapper = mapper;
            _logger = logger;
            _ticketLinkService = ticketLinkService;
        }

        /// <summary>
        /// Returns linked tickets for ticket with ticketId
        /// </summary>
        /// <param name="teamId"></param>
        /// <param name="ticketId">Ticket ticketId</param>
        [Route("user/teams/{teamId}/tickets/{ticketId}/unlinked")]
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(HandleErrorInfo), Description = "Ticket ticketId does not have value")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(TicketApiModel), Description = "A list of tags")]
        public async Task<IActionResult> GetUnLinkedTickets(Guid teamId, Guid? ticketId)
        {
            if (!ticketId.HasValue)
            {
                return BadRequest();
            }

            var linkedTicketDtos = await _ticketLinkService.GetUnlinkedTicketsAsync(teamId, ticketId.Value);
            var linkedTicketApiModels = _mapper.Map<IEnumerable<TicketApiModel>>(linkedTicketDtos);

            _logger.LogInformation($"UnLinked tickets were successfully got by ticketId: {ticketId}");

            return Json(linkedTicketApiModels);
        }

        /// <summary>
        /// Returns linked tickets for ticket with ticketId
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="ticketId">Ticket ticketId</param>
        [Route("user/teams/{teamId}/tickets/{ticketId}/linked")]
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(HandleErrorInfo), Description = "Ticket ticketId does not have value")]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(TicketApiModel), Description = "A list of tags")]
        public async Task<IActionResult> GetLinkedTickets(Guid teamId, Guid? ticketId)
        {
            if (!ticketId.HasValue)
            {
                return BadRequest();
            }

            var linkedTicketDtos = await _ticketLinkService.GetLinkedTicketsAsync(teamId, ticketId.Value);
            var linkedTicketApiModels = _mapper.Map<IEnumerable<TicketApiModel>>(linkedTicketDtos);

            _logger.LogInformation($"Linked tickets were successfully got by ticketId: {ticketId}");

            return Json(linkedTicketApiModels);
        }

        /// <summary>
        /// Links two tickets
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="sourceTicket">Source ticket ticketId</param>
        /// <param name="destinationTicket">Destination ticket ticketId</param>
        [Route("user/teams/{teamId}/tickets/{sourceTicket}/linked/{destinationTicket}")]
        [HttpPut]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(HandleErrorInfo), Description = "Source or destination ticket ticketId does not have value")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(HandleErrorInfo), Description = "Ticket with such ticketId does not exist")]
        public async Task<IActionResult> LinkTickets(Guid teamId, Guid? sourceTicket, Guid? destinationTicket)
        {
            if (!sourceTicket.HasValue || !destinationTicket.HasValue)
            {
                return BadRequest();
            }

            await _ticketLinkService.LinkTicketsAsync(teamId, sourceTicket.Value, destinationTicket.Value);

            _logger.LogInformation($"Tickets {sourceTicket} and {destinationTicket} were linked");

            return Ok();
        }

        /// <summary>
        /// Unlinks two tickets
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="sourceTicket"></param>
        /// <param name="destinationTicket"></param>
        [Route("user/teams/{teamId}/tickets/{sourceTicket}/linked/{destinationTicket}")]
        [HttpDelete]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(HandleErrorInfo), Description = "Source or destination ticket ticketId does not have value")]
        [SwaggerResponse((int)HttpStatusCode.NotFound, typeof(HandleErrorInfo), Description = "Ticket with such ticketId does not exist")]
        public async Task<IActionResult> UnLinkTickets(Guid teamId, Guid? sourceTicket, Guid? destinationTicket)
        {
            if (!sourceTicket.HasValue || !destinationTicket.HasValue)
            {
                return BadRequest();
            }

            await _ticketLinkService.UnlinkTicketsAsync(teamId, sourceTicket.Value, destinationTicket.Value);

            _logger.LogInformation($"Tickets {sourceTicket} and {destinationTicket} were unlinked");

            return Ok();
        }
    }
}
