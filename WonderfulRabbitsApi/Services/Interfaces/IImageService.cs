using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models.Images;

namespace WonderfulRabbitsApi.Services.Interfaces;

public interface IImageService
{
    Task<int> RegisterImageAsync(RegisterImageModel model);
    Task<Image> GetImageAsync(int id);
    Task<List<Image>> GetImageAsync();
}