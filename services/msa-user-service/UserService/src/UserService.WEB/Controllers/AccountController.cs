﻿using System.Net;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.SwaggerGen;
using UserService.BLL.DTO;
using UserService.BLL.Interfaces;
using UserService.WEB.Models.AccountApiModels;

namespace UserService.WEB.Controllers
{
    [Route("UserService/api/")]
    public class AccountController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        private readonly ILogger<AccountController> _logger;

        public AccountController(IAccountService accountService, IMapper mapper, ILogger<AccountController> logger)
        {
            _accountService = accountService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Registers a new user
        /// </summary>
        /// <param name="model">Contains user's login, password, name</param>
        [Route("register")]
        [HttpPost]
        [SwaggerResponse((int)HttpStatusCode.OK, Description = "Success")]
        [SwaggerResponse((int)HttpStatusCode.BadRequest, Description = "Invalid Model")]
        [SwaggerResponse((int)HttpStatusCode.InternalServerError, Description = "Internal server exception")]
        public async Task<IActionResult> Register([FromBody] RegisterApiModel model)
        {
            if (ModelState.IsValid)
            {
                var registerModelDto = _mapper.Map<RegisterModelDto>(model);
                await _accountService.RegisterAsync(registerModelDto);

                _logger.LogInformation($"User was registration with email: {model.Email}");

                return Ok();
            }

            return BadRequest(ModelState);
        }
    }
}
