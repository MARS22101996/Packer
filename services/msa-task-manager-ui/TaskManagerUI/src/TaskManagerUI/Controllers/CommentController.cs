using System;
using System.Threading.Tasks;
using AutoMapper;
using EpamMA.Communication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TaskManagerUI.ApiModels;
using TaskManagerUI.Infrastructure.Authorization;
using TaskManagerUI.ViewModels.CommentViewModels;

namespace TaskManagerUI.Controllers
{
    [Authorize]
    public class CommentController : BaseController
    {
        private readonly ICommunicationService _communicationService;
        private readonly IMapper _mapper;

        public CommentController(IMapper mapper, ICommunicationService communicationService)
        {
            _mapper = mapper;
            _communicationService = communicationService;
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody]CommentViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var comment = _mapper.Map<CommentApiModel>(model);
            comment.User = await _communicationService.GetAsync<UserApiModel>($"users/{User.GetUserId()}", null, FormHeaders(JsonType), "userapi");

            await _communicationService
                .PostAsync($"user/teams/{model.TeamId}/tickets/{model.TicketId}/comments", comment, FormHeaders(JsonType), "ticketapi");

            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> Delete(Guid? commentId, Guid? ticketId, Guid? teamId)
        {
            if (!teamId.HasValue || !commentId.HasValue || !ticketId.HasValue)
            {
                return BadRequest();
            }

            await _communicationService
                .DeleteAsync($"user/teams/{teamId}/tickets/{ticketId}/comments/{commentId}", null, FormHeaders(JsonType), "ticketapi");

            return Ok();
        }
    }
}