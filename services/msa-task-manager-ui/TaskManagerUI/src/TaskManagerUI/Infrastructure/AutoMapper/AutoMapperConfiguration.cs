using AutoMapper;

namespace TaskManagerUI.Infrastructure.AutoMapper
{
    public class AutoMapperConfiguration
    {
        public MapperConfiguration Configure()
        {
            var mapperConfiguration = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ApiModelToViewModelProfile>();
                cfg.AddProfile<ViewModelToApiModelProfile>();
            });

            return mapperConfiguration;
        }
    }
}
