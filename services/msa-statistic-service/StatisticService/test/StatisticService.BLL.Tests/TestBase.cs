using AutoMapper;
using StatisticService.WEB.Infrastructure.Automapper;

namespace StatisticService.BLL.Tests
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