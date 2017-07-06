using AutoMapper;

namespace StatisticService.WEB.Infrastructure.Automapper
{
    public class AutoMapperConfiguration
    {
        public MapperConfiguration Configure()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoToApiModelProfile>();
                cfg.AddProfile<ApiModelToDtoProfile>();
            });

            return mapperConfiguration;
        }
    }
}