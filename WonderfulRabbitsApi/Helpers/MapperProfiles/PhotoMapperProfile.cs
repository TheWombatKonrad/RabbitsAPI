using AutoMapper;
using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models.Photos;
using WonderfulRabbitsApi.Models.Rabbits;

namespace WonderfulRabbitsApi.Helpers.MapperProfiles
{
    public class PhotoMapperProfile : Profile
    {
        public PhotoMapperProfile()
        {
            CreateMap<RegisterPhotoModel, Photo>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Rabbit, opt => opt.Ignore())
            .ForMember(x => x.DateAdded, opt => opt.Ignore())
            .ForSourceMember(x => x.RabbitId, opt => opt.DoNotValidate());
        }
    }
}