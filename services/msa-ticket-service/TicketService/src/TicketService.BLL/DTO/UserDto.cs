﻿using System;

namespace TicketService.BLL.DTO
{
    public class UserDto
    {
        public Guid Id { get; set; }

        public string Email { get; set; }

        public string Name { get; set; }
    }
}