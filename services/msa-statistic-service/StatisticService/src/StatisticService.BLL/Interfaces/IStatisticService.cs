using System;
using System.Collections.Generic;
using StatisticService.BLL.DTO;

namespace StatisticService.BLL.Interfaces
{
    public interface IStatisticService
    {
        UserStatisticDto GetStatisticFiltered(DateTime startDate, List<TicketDto> tickets);
    }
}