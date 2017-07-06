using System;
using System.Threading.Tasks;
using EpamMA.Communication.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TaskManagerUI.Models;
using TaskManagerUI.ViewModels.AccountViewModels;

namespace TaskManagerUI.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ICommunicationService _communicationService;
        private const string CookieTokenKeyName = "token";

        public AccountController(ICommunicationService communicationService)
        {
            _communicationService = communicationService;
        }


        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await SetTokenCookie(model.Email, model.Password);

            return RedirectToAction("Board", "Ticket");
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _communicationService.PostAsync("register", model, FormHeaders(JsonType), "userapi");
            await SetTokenCookie(model.Email, model.Password);

            return RedirectToAction("Board", "Ticket");
        }

        [HttpGet]
        [Authorize]
        public IActionResult LogOff()
        {
            Response.Cookies.Delete(CookieTokenKeyName);

            return RedirectToAction("Board", "Ticket");
        }

        private async Task SetTokenCookie(string email, string password)
        {
            var body = $"username={email}&password={password}";
            var token = await _communicationService.PostAsync<TokenApiModel, string>("token", body, FormHeaders(FormType));

            Response.Cookies.Append(CookieTokenKeyName, token.Token, new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(token.ExpiresIn)
            });
        }
    }
}