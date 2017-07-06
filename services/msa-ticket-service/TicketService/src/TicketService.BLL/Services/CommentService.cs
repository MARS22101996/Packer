using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.Logging;
using TicketService.BLL.DTO;
using TicketService.BLL.Interfaces;
using TicketService.DAL.Entities;
using TicketService.DAL.Interfaces;

namespace TicketService.BLL.Services
{
    public class CommentService : ICommentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<CommentService> _logger;

        public CommentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<CommentService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<IEnumerable<CommentDto>> GetAllAsync(Guid teamId, Guid ticketId)
        {
            var comments = await _unitOfWork.Comments.GetAllAsync(teamId, ticketId);
            var commentDtos = _mapper.Map<List<CommentDto>>(comments);

            _logger.LogInformation($"Comments were successfully received by ticketId: {ticketId}");

            return commentDtos;
        }

        public async Task<Guid> CreateAsync(Guid teamId, Guid ticketId, CommentDto commentDto)
        {
            commentDto.Date = DateTime.UtcNow;

            var comment = _mapper.Map<Comment>(commentDto);

            var createdCommentId = await _unitOfWork.Comments.CreateAsync(teamId, ticketId, comment);

            _logger.LogInformation($"Comment with text {commentDto.Text} was successfully created");

            return createdCommentId;
        }

        public async Task DeleteAsync(Guid teamId, Guid ticketId, Guid id)
        {
            await _unitOfWork.Comments.DeleteAsync(teamId, ticketId, id);

            _logger.LogInformation($"Comment {id} was successfully deleted");
        }
    }
}