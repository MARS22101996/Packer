using System.Collections.Generic;
using EpamMA.ReverseProxy.Entities;

namespace ApiGateway.ViewModels
{
    public class DashboardViewModel
    {
        public List<Path> Paths { get; set; }

        public LoadBalancer LoadBalancer { get; set; }
    }
}