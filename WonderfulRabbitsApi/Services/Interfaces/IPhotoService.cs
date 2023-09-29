using WonderfulRabbitsApi.Models.Photos;

namespace WonderfulRabbitsApi.Services.Interfaces;

public interface IPhotoService
{
    Task<int> RegisterPhoto(RegisterPhotoModel model);
}