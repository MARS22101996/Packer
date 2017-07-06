using AutoMapper;
using NotificationService.BLL.Infrastructure.Automapper;

namespace NotificationService.WEB.Infrastructure.Automapper
{
    public class AutoMapperConfiguration
    {
        public MapperConfiguration Configure()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<DtoToViewModelProfile>();
                cfg.AddProfile<ViewModelToDtoProfile>();

                ServiceAutoMapperConfiguration.Initialize(cfg);
            });

            return mapperConfiguration;
        }
    }
}