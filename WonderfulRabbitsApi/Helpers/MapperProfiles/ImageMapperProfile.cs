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
            CreateMap<RegisterImageModel, Image>()
            .ForMember(x => x.Id, opt => opt.Ignore())
            .ForMember(x => x.Rabbit, opt => opt.Ignore())
            .ForMember(x => x.DateAdded, opt => opt.Ignore())
            .ForSourceMember(x => x.RabbitId, opt => opt.DoNotValidate());

            CreateMap<Image, ImageModel>();

            CreateMap<string, byte[]>().ConvertUsing(s => Convert.FromBase64String(s));
            CreateMap<byte[], string>().ConvertUsing(bytes => Convert.ToBase64String(bytes));
        }
    }
}