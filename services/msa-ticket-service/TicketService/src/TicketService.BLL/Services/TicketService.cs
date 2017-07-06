using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using AutoMapper;
using LinqKit;
using Microsoft.Extensions.Logging;
using TicketService.BLL.DTO;
using TicketService.BLL.Infrastructure.Exceptions;
using TicketService.BLL.Interfaces;
using TicketService.Core.Enums;
using TicketService.DAL.Entities;
using TicketService.DAL.Interfaces;

namespace TicketService.BLL.Services
{
    public class TicketService : ITicketService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ITagService _tagService;
        private readonly ITicketLinkService _ticketLinkService;
        private readonly IMapper _mapper;
        private readonly ILogger<TicketService> _logger;

        public TicketService(
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ITagService tagService,
            ITicketLinkService ticketLinkService,
            ILogger<TicketService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _tagService = tagService;
            _ticketLinkService = ticketLinkService;
            _logger = logger;
        }

        public IEnumerable<TicketDto> GetAll(Guid teamId)
        {
            var tickets = _unitOfWork.Tickets.GetAll(teamId);
            var ticketDtos = _mapper.Map<IEnumerable<TicketDto>>(tickets);

            _logger.LogInformation("Tickets were successfully received");

            return ticketDtos;
        }

        public async Task<int> GetCountAsync(Guid teamId, FilterDto filterDto)
        {
            var filterExpression = CreateSortingExpression(filterDto);
            var tickets = await _unitOfWork.Tickets.GetCountAsync(teamId, filterExpression);

            _logger.LogInformation("Ticket count was successfully received");

            return tickets;
        }

        public async Task<IEnumerable<TicketDto>> GetAllFilteredAsync(Guid teamId, FilterDto filterDto)
        {
            var filterExpression = CreateSortingExpression(filterDto);

            var skipTickets = (filterDto.Page - 1) * filterDto.PageSize;
            var takeTickets = filterDto.PageSize;

            var tickets = await _unitOfWork.Tickets.FindAsync(teamId, skipTickets, takeTickets, filterExpression);
            var ticketDtos = _mapper.Map<IEnumerable<TicketDto>>(tickets);

            _logger.LogInformation("Filtered tickets were successfully received");

            return ticketDtos;
        }

        public async Task<TicketDto> GetAsync(Guid teamId, Guid id)
        {
            var ticket = await _unitOfWork.Tickets.GetAsync(teamId, id);

            if (ticket == null)
            {
                throw new EntityNotFoundException($"Ticket with such id does not exist. Id: {id}", "Ticket");
            }

            var ticketDto = _mapper.Map<TicketDto>(ticket);

            _logger.LogInformation($"Ticket was successfully received by id: {id}");

            return ticketDto;
        }

        public async Task<Guid> CreateAsync(Guid teamId, TicketDto ticketDto)
        {
            var ticket = _mapper.Map<Ticket>(ticketDto);
            await _tagService.AddAsync(ticketDto.Tags);
            ticket.CreationDate = DateTime.UtcNow;

            var id = await _unitOfWork.Tickets.CreateAsync(teamId, ticket);

            _logger.LogInformation($"Ticket with name {ticket.Name} was successfully created");

            return id;
        }

        public async Task UpdateAsync(Guid teamId, TicketDto ticketDto)
        {
            var ticketToUpdate = _mapper.Map<Ticket>(ticketDto);
            await _tagService.AddAsync(ticketDto.Tags);

            await _unitOfWork.Tickets.UpdateAsync(teamId, ticketToUpdate);

            var tickets = await _unitOfWork.Tickets.GetAll(teamId);
            foreach (var ticket in tickets)
            {
                if (ticketDto.LinkedTicketIds.Contains(ticket.Id) && !ticket.LinkedTicketIds.Contains(ticketDto.Id))
                {
                    ticket.LinkedTicketIds = ticket.LinkedTicketIds.Append(ticketDto.Id);
                    await _unitOfWork.Tickets.UpdateAsync(teamId, ticket);
                }
                else if (!ticketDto.LinkedTicketIds.Contains(ticket.Id) && ticket.LinkedTicketIds.Contains(ticketDto.Id))
                {
                    ticket.LinkedTicketIds.ToList().Remove(ticketDto.Id);
                    await _unitOfWork.Tickets.UpdateAsync(teamId, ticket);
                }
            }

            _logger.LogInformation($"Ticket with id {ticketDto.Id}  was successfully updated ");
        }

        public async Task DeleteAsync(Guid teamId, Guid id)
        {
            var ticket = await _unitOfWork.Tickets.GetAsync(teamId, id);

            if (ticket == null)
            {
                throw new EntityNotFoundException($"Ticket with such id does not exist. Id: {id}", "Ticket");
            }

            var unlinkTicketsTasks = ticket.LinkedTicketIds
                .Select(linkedTicketId => _ticketLinkService.UnlinkTicketsAsync(teamId, id, linkedTicketId))
                .ToArray();

            await Task.WhenAll(unlinkTicketsTasks);

            await _unitOfWork.Tickets.DeleteAsync(teamId, id);

            _logger.LogInformation($"Ticket with id {id} was successfully deleted");
        }

        public async Task UpdateStatusAsync(Guid teamId, Guid id, Status status)
        {
            var ticket = await _unitOfWork.Tickets.GetAsync(teamId, id);

            if (ticket == null)
            {
                throw new EntityNotFoundException($"Ticket with such id does not exist. Id: {id}", "Ticket");
            }

            ticket.Status = status;

            await _unitOfWork.Tickets.UpdateAsync(teamId, ticket);

            _logger.LogInformation($"Status of ticket with id {id} was successfully updated to {status}");
        }

        private Expression<Func<Ticket, bool>> CreateSortingExpression(FilterDto input)
        {
            Expression<Func<Ticket, bool>> expression = p => true;

            if (input.DateFrom.HasValue && input.DateTo.HasValue)
            {
                expression = expression
                    .And(ticket => ticket.CreationDate >= input.DateFrom && ticket.CreationDate <= input.DateTo);
            }

            if (!input.DateFrom.HasValue && input.DateTo.HasValue)
            {
                expression = expression
                    .And(p => p.CreationDate >= DateTime.MinValue && p.CreationDate <= input.DateTo);
            }

            if (input.DateFrom.HasValue && !input.DateTo.HasValue)
            {
                expression = expression
                    .And(p => p.CreationDate >= input.DateFrom && p.CreationDate <= DateTime.UtcNow);
            }

            if (input.SelectedStatuses.Any())
            {
                expression = expression
                    .And(p => input.SelectedStatuses.Contains(p.Status));
            }

            if (input.SelectedPriorities.Any())
            {
                expression = expression
                    .And(p => input.SelectedPriorities.Contains(p.Priority));
            }

            return expression.Expand();
        }
    }
}