using System;
using System.Collections.Generic;

namespace TeamService.BLL.DTO
{
    public class TeamDto
    {
        public TeamDto()
        {
            Participants = new List<UserDto>();
        }

        public Guid Id { get; set; }

        public string Name { get; set; }

        public UserDto Owner { get; set; }

        public IEnumerable<UserDto> Participants { get; set; }
    }
}