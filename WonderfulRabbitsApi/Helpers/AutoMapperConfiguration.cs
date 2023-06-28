using AutoMapper;
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
            });

            return config;
        }
    }
}