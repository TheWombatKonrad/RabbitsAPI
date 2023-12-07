using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models.Images;

namespace WonderfulRabbitsApi.Services.Interfaces;

public interface IImageService
{
    Task<int> UploadImageAsync(UploadImageModel model);
    Task<Image> GetImageAsync(int id);
    Task<List<Image>> GetImagesAsync();
    Task DeleteImageAsync(int id);
    Task UpdateImageAsync(int id, UpdateImageModel model);
}