using AutoMapper;
using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models.Rabbits;

namespace WonderfulRabbitsApi.Helpers.MapperProfiles
{
    public class RabbitMapperProfile : Profile
    {
        public RabbitMapperProfile()
        {
            CreateMap<Rabbit, RabbitModel>();

            CreateMap<RegisterRabbitModel, Rabbit>()
                .ForMember(x => x.Id, act => act.Ignore())
                .ForMember(x => x.User, act => act.Ignore())
                .ReverseMap();




        }
    }
}