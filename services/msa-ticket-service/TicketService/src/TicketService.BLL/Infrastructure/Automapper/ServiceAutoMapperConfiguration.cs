using AutoMapper;

namespace TicketService.BLL.Infrastructure.Automapper
{
    public class ServiceAutoMapperConfiguration
    {
        public static void Initialize(IMapperConfigurationExpression cfg)
        {
            cfg.AddProfile(new DTOToEntityProfile());
            cfg.AddProfile(new EntityToDTOProfile());
        }
    }
}