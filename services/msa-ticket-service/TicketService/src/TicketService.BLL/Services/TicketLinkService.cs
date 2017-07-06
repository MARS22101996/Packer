using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TicketService.BLL.DTO;
using TicketService.BLL.Infrastructure.Exceptions;
using TicketService.BLL.Interfaces;
using TicketService.DAL.Entities;
using TicketService.DAL.Interfaces;

namespace TicketService.BLL.Services
{
    public class TicketLinkService : ITicketLinkService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketLinkService> _logger;

        public TicketLinkService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TicketLinkService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<TicketDto>> GetLinkedTicketsAsync(Guid teamId, Guid id)
        {
            var expression = BuildExpressionForLinkedTickets(id);

            var tickets = await _unitOfWork.Tickets.FindAsync(teamId, expression);
            var ticketDtos = _mapper.Map<List<TicketDto>>(tickets);

            _logger.LogInformation($"Linked tickets were successfully received by id: {id}");

            return ticketDtos;
        }

        public async Task<IEnumerable<TicketDto>> GetUnlinkedTicketsAsync(Guid teamId, Guid id)
        {
            var expression = BuildExpressionForUnLinkedTickets(id);

            var tickets = await _unitOfWork.Tickets.FindAsync(teamId, expression);
            var ticketDtos = _mapper.Map<List<TicketDto>>(tickets);

            _logger.LogInformation($"Unlinked tickets were successfully received by id: {id}");

            return ticketDtos;
        }

        public async Task UnlinkTicketsAsync(Guid teamId, Guid sourceTicketId, Guid destinationTicketId)
        {
            var tickets = await GetTicketsByIds(teamId, sourceTicketId, destinationTicketId);

            var sourceTicket = tickets.FirstOrDefault(ticket => ticket.Id == sourceTicketId);
            var destinationTicket = tickets.FirstOrDefault(ticket => ticket.Id == destinationTicketId);

            sourceTicket.LinkedTicketIds = sourceTicket.LinkedTicketIds.Where(id => id != destinationTicketId);
            destinationTicket.LinkedTicketIds = destinationTicket.LinkedTicketIds.Where(id => id != sourceTicketId);

            await _unitOfWork.Tickets.UpdateAsync(teamId, sourceTicket);

            _logger.LogInformation($"Ticket with id {destinationTicketId} was successfully removed from linked tickets of ticket {sourceTicketId}");

            await _unitOfWork.Tickets.UpdateAsync(teamId, destinationTicket);

            _logger.LogInformation($"Ticket with id {sourceTicketId} was successfully removed from linked tickets of ticket {destinationTicketId}");
        }

        public async Task LinkTicketsAsync(Guid teamId, Guid sourceTicketId, Guid destinationTicketId)
        {
            var tickets = await GetTicketsByIds(teamId, sourceTicketId, destinationTicketId);

            var sourceTicket = tickets.FirstOrDefault(ticket => ticket.Id == sourceTicketId);
            var destinationTicket = tickets.FirstOrDefault(ticket => ticket.Id == destinationTicketId);

            sourceTicket.LinkedTicketIds = sourceTicket.LinkedTicketIds.Append(destinationTicketId);
            destinationTicket.LinkedTicketIds = destinationTicket.LinkedTicketIds.Append(sourceTicketId);

            await _unitOfWork.Tickets.UpdateAsync(teamId, sourceTicket);

            _logger.LogInformation($"Ticket with id {destinationTicketId} was successfully added to linked tickets of ticket {sourceTicketId}");

            await _unitOfWork.Tickets.UpdateAsync(teamId, destinationTicket);

            _logger.LogInformation($"Ticket with id {sourceTicketId} was successfully added to linked tickets of ticket {destinationTicketId}");
        }

        private async Task<IQueryable<Ticket>> GetTicketsByIds(Guid teamId, params Guid[] ticketIds)
        {
            var tickets = (await _unitOfWork.Tickets.FindAsync(teamId, ticket => ticketIds.Contains(ticket.Id))).ToList();

            var foundTicketIds = tickets.Select(ticket => ticket.Id);
            var missingIds = ticketIds.Where(guid => !foundTicketIds.Contains(guid)).ToList();

            if (missingIds.Any())
            {
                var message = missingIds
                    .Aggregate("Tickets with such ids do not exist. Ids: ", (current, id) => current + id.ToString() + ", ");

                throw new EntityNotFoundException(message, "Ticket");
            }

            return tickets.AsQueryable();
        }

        private Expression<Func<Ticket, bool>> BuildExpressionForLinkedTickets(Guid id)
        {
            Expression<Func<Ticket, bool>> expression =
                ticket => ticket.LinkedTicketIds.Contains(id) && ticket.Id != id;

            return expression;
        }

        private Expression<Func<Ticket, bool>> BuildExpressionForUnLinkedTickets(Guid id)
        {
            Expression<Func<Ticket, bool>> expression =
                ticket => !ticket.LinkedTicketIds.Contains(id) && ticket.Id != id;

            return expression;
        }
    }
}