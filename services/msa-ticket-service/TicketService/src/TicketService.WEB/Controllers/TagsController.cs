using System.Collections.Generic;
using System.Net;
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
    public class TagsController : Controller
    {
        private readonly ITagService _tagService;
        private readonly IMapper _mapper;
        private readonly ILogger<TagsController> _logger;

        public TagsController(ITagService tagService, IMapper mapper, ILogger<TagsController> logger)
        {
            _tagService = tagService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Returns all tags
        /// </summary>
        [Route("tags")]
        [HttpGet]
        [SwaggerResponse((int)HttpStatusCode.OK, typeof(IEnumerable<TagApiModel>), Description = "A list of tags")]
        public IActionResult GetTags()
        {
            var tagDtos = _tagService.GetAll();
            var tagApiModels = _mapper.Map<IEnumerable<TagApiModel>>(tagDtos);

            _logger.LogInformation("Tags were successfully received");

            return Json(tagApiModels);
        }
    }
}