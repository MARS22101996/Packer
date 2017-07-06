using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using EpamMA.Communication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using TicketService.BLL.DTO;
using TicketService.BLL.Interfaces;
using TicketService.WEB.Models;

namespace TicketService.WEB.Controllers
{
    [Authorize]
    [Route("TicketService/api")]
    public class CommentsController : BaseController
    {
        private readonly IMapper _mapper;
        private readonly ICommentService _commentService;
        private readonly ILogger<CommentsController> _logger;

        public CommentsController(
            IMapper mapper,
            ICommentService commentService,
            ILogger<CommentsController> logger,
            ICommunicationService communicationService)
        {
            _mapper = mapper;
            _commentService = commentService;
            _logger = logger;
        }

        /// <summary>
        /// Gets comment from ticket
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="id">Comment model</param>
        [Route("user/teams/{teamId}/tickets/{id}/comments")]
        [HttpGet]
        public async Task<IActionResult> Get(Guid teamId, Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest();
            }

            var commentDtos = await _commentService.GetAllAsync(teamId, id);
            var commentApiModels = _mapper.Map<IEnumerable<CommentApiModel>>(commentDtos);

            _logger.LogInformation("Comments were succesfully returned");

            return Json(commentApiModels);
        }

        /// <summary>
        /// Adds comment to ticket
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="id">Comment model</param>
        /// <param name="model">Comment model</param>
        [Route("user/teams/{teamId}/tickets/{id}/comments")]
        [HttpPost]
        public async Task<IActionResult> Post(Guid teamId, Guid id, [FromBody]CommentApiModel model)
        {
            if (ModelState.IsValid)
            {
                var commentDto = _mapper.Map<CommentDto>(model);
                var createdCommentId = await _commentService.CreateAsync(teamId, id, commentDto);

                _logger.LogInformation($"Comment was succesfully created with text {model.Text}");
                return Ok(createdCommentId);
            }

            return BadRequest();
        }

        /// <summary>
        /// Deletes comment from ticket
        /// </summary>
        /// <param name="teamId">Team Id</param>
        /// <param name="commentId">Comment id</param>
        /// <param name="ticketId">Ticket id</param>
        [Route("user/teams/{teamId}/tickets/{ticketId}/comments/{commentId}")]
        [HttpDelete]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, typeof(HandleErrorInfo), Description = "Comment or ticket id does not have value")]
        public async Task<IActionResult> Delete(Guid teamId, Guid? ticketId, Guid? commentId)
        {
            if (!commentId.HasValue || !ticketId.HasValue)
            {
                return BadRequest();
            }

            await _commentService.DeleteAsync(teamId, ticketId.Value, commentId.Value);

            _logger.LogInformation($"Comment {commentId} for ticket {ticketId} was successfully deleted");

            return Ok();
        }
    }
}