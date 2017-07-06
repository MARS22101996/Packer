using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using NotificationService.BLL.DTO;
using NotificationService.BLL.Infrastructure;
using NotificationService.BLL.Infrastructure.Exceptions;
using NotificationService.BLL.Interfaces;
using NotificationService.DAL.Entities;
using NotificationService.DAL.Interfaces;

namespace NotificationService.BLL.Services
{
    public class NotificationService : INotificationService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IEmailSender _emailSender;
        private readonly IMapper _mapper;
        private readonly ILogger<NotificationService> _logger;

        public NotificationService(
            IUnitOfWork unitOfWork,
            IEmailSender emailSender,
            IMapper mapper,
            ILogger<NotificationService> logger)
        {
            _unitOfWork = unitOfWork;
            _emailSender = emailSender;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<UserDto> GetWatcherAsync(Guid teamId, Guid ticketId, Guid userId)
        {
            var watcher = await _unitOfWork.Watchers.GetAsync(teamId, ticketId, userId);

            if (watcher == null)
            {
                throw new EntityNotFoundException(
                    $"User doesn't watch for this ticket. Ticket id: {ticketId}, user id: {userId}",
                    "User");
            }

            var userDto = _mapper.Map<UserDto>(watcher);

            _logger.LogInformation($"Get watcher with id: {userId} for ticket with id: {ticketId}");

            return userDto;
        }

        public async Task<IEnumerable<UserDto>> GetAllWatchersAsync(Guid teamId, Guid ticketId)
        {
            var watchers = await _unitOfWork.Watchers.GetAllAsync(teamId, ticketId);
            var userDtos = _mapper.Map<IEnumerable<UserDto>>(watchers);

            _logger.LogInformation($"Get watchers for ticket with id: {ticketId}");

            return userDtos;
        }

        public async Task AddWatcherAsync(Guid ticketId, UserDto userDto)
        {
            var watcher = _mapper.Map<User>(userDto);
            await _unitOfWork.Watchers.AddAsync(ticketId, watcher);

            _logger.LogInformation($"Add watcher with id: {userDto.Id} for ticket with id: {ticketId}");
        }

        public async Task RemoveWatcherAsync(Guid teamId, Guid ticketId, Guid userId)
        {
            var watcher = await _unitOfWork.Watchers.GetAsync(teamId, ticketId, userId);

            if (watcher == null)
            {
                throw new EntityNotFoundException(
                    $"User doesn't watch for this ticket. Ticket id: {ticketId}, user id: {userId}",
                    "Notification");
            }

            await _unitOfWork.Watchers.RemoveAsync(ticketId, userId);

            _logger.LogInformation($"Remove watcher with id: {userId} from ticket with id: {ticketId}");
        }

        public async Task NotifyTicketWatchersAsync(Guid teamId, Guid ticketId, NotificationInfo info)
        {
            var message = FormMessage(info);
            var watchersToNotify = await _unitOfWork.Watchers.GetAllAsync(teamId, ticketId);

            var emails = watchersToNotify.Select(user => user.Email);

            await _emailSender.SendAsync(emails, message, "TaskManager notification");

            _logger.LogInformation($"Notify watchers with for ticket with id: {ticketId}");
        }

        private static string FormMessage(NotificationInfo info)
        {
            var message = MessageFormater.FormNotificationMassage(info);

            return message;
        }
    }
}