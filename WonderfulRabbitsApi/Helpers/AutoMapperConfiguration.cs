using AutoMapper;
using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Helpers.MapperProfiles;

namespace WonderfulRabbitsApi.Helpers
{
    public static class AutoMapperConfiguration
    {
        public static MapperConfiguration Configure()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<UserMapperProfile>();
                cfg.AddProfile<RabbitMapperProfile>();
                cfg.AddProfile<PhotoMapperProfile>();
            });

            return config;
        }
    }
}