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
                .ForMember(x => x.Id, opt => opt.Ignore())
                .ForMember(x => x.User, opt => opt.Ignore())
                .ForSourceMember(x => x.UserId, opt => opt.DoNotValidate());

            CreateMap<Rabbit, RegisterRabbitModel>()
                .ForMember(x => x.UserId, opt => opt.Ignore())
                .ForMember(x => x.Photos, opt => opt.Ignore());


            CreateMap<UpdateRabbitModel, Rabbit>(MemberList.Source)
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        return true;
                    }
                ));


        }
    }
}