using AutoMapper;
using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models;
using WonderfulRabbitsApi.Models.Images;
using WonderfulRabbitsApi.Models.Rabbits;

namespace WonderfulRabbitsApi.Helpers.MapperProfiles
{
    public class ImageMapperProfile : Profile
    {
        public ImageMapperProfile()
        {
            CreateMap<UploadImageModel, Image>()
            .ForMember(x => x.ImageData, opt => opt.MapFrom(x => x.Base64ImageData))
            .ForMember(x => x.FileName, opt => opt.Ignore())
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Rabbit, opt => opt.Ignore())
            .ForMember(x => x.DateAdded, opt => opt.Ignore())
            .ForSourceMember(x => x.RabbitId, opt => opt.DoNotValidate())
            .ReverseMap();

            CreateMap<Image, ImageModel>()
                .ForMember(x => x.Base64ImageData, opt => opt.MapFrom(x => x.ImageData))
                .ReverseMap();

            CreateMap<UpdateImageModel, Image>(MemberList.Source)
                .ForAllMembers(x => x.Condition(
                    (src, dest, prop) =>
                    {
                        // ignore null & empty string properties
                        if (prop == null) return false;
                        if (prop.GetType() == typeof(string) && string.IsNullOrEmpty((string)prop)) return false;

                        return true;
                    }
                ));

            CreateMap<string, byte[]>().ConvertUsing(s => Convert.FromBase64String(s));
            CreateMap<byte[], string>().ConvertUsing(bytes => Convert.ToBase64String(bytes));
        }
    }
}