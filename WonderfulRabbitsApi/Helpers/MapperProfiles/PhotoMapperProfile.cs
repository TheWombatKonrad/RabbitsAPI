using AutoMapper;
using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models;
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
            .ForMember(x => x.ImageData, opt => opt.Ignore())
            .ForSourceMember(x => x.RabbitId, opt => opt.DoNotValidate())
            .ForSourceMember(x => x.Base64ImageData, opt => opt.DoNotValidate());

            CreateMap<Photo, PhotoModel>()
                .ForMember(x => x.ImageData, opt => opt.Ignore());
        }
    }
}