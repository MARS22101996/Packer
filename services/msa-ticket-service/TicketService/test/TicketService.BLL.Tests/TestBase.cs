using AutoMapper;
using TicketService.WEB.Infrastructure.AutoMapper;

namespace TicketService.BLL.Tests
{
    public class TestBase
    {
        protected IMapper Mapper { get; set; }

        protected void SetUp()
        {
            var config = new AutoMapperConfiguration();
            Mapper = config.Configure().CreateMapper();
        }
    }
}