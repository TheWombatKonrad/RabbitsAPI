using AutoMapper;
using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models.Users;

namespace WonderfulRabbitsApi.Helpers.MapperProfiles
{
    public class UserMapperProfile : Profile
    {
        public UserMapperProfile()
        {
            CreateMap<User, AuthenticateResponse>().ForMember(d => d.Token, act => act.Ignore());
            CreateMap<User, UserModel>();
            CreateMap<RegisterUserModel, User>(MemberList.Source).ForSourceMember(s => s.Password, act => act.DoNotValidate());

            CreateMap<UpdateUser, User>(MemberList.Source)
                .ForSourceMember(s => s.Password, act => act.DoNotValidate())
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