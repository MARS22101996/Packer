using System;

namespace StatisticService.WEB.Models.StatisticApiModels
{
    public class UserApiModel
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
    }
}