using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models.Photos;

namespace WonderfulRabbitsApi.Services.Interfaces;

public interface IPhotoService
{
    Task<int> RegisterPhotoAsync(RegisterPhotoModel model);
    Task<Photo> GetPhotoAsync(int id);
    Task<List<Photo>> GetPhotosAsync();
}