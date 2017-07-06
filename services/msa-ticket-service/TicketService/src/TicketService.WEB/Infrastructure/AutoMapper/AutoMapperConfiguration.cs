using AutoMapper;
using TicketService.BLL.Infrastructure.Automapper;

namespace TicketService.WEB.Infrastructure.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public MapperConfiguration Configure()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoToApiModelProfile>();
                cfg.AddProfile<ApiModelToDtoProfile>();

                ServiceAutoMapperConfiguration.Initialize(cfg);
            });

            return mapperConfiguration;
        }
    }
}