using ApiGateway.ViewModels;
using EpamMA.ReverseProxy.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace ApiGateway.Controllers
{
    public class DashboardController : Controller
    {
        private ProxyConfiguration ProxyConfiguration { get; }

        public DashboardController(IOptionsSnapshot<ProxyConfiguration> appConfiguration)
        {
            ProxyConfiguration = appConfiguration.Value;
        }

        public IActionResult Index()
        {
            var dashboarViewModel = new DashboardViewModel
            {
                Paths = ProxyConfiguration.Paths,
                LoadBalancer = ProxyConfiguration.LoadBalancer
            };

            return View(dashboarViewModel);
        }

        public IActionResult AddPath(string from, string to)
        {
            if (string.IsNullOrEmpty(from) || string.IsNullOrEmpty(to))
            {
                return View("Error", "Invalid input");
            }

            ProxyConfiguration.AddPath(from, to);

            return RedirectToAction("Index");
        }

        public IActionResult RemovePath(string from, string to)
        {
            if (from == null || to == null)
            {
                return View("Error");
            }

            ProxyConfiguration.RemovePath(new Path {From = from, To = to});

            return RedirectToAction("Index");
        }
    }
}