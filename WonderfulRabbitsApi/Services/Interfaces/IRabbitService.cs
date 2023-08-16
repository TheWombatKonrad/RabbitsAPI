
using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models.Rabbits;

namespace WonderfulRabbitsApi.Services.Interfaces;
public interface IRabbitService
{
    Task<int> RegisterRabbitAsync(RegisterRabbitModel model);
    Task<Rabbit> GetRabbitAsync(int id);
    Task<List<Rabbit>> GetRabbitsAsync();
    Task UpdateRabbitAsync(int id, UpdateRabbitModel model);
    Task DeleteRabbitAsync(int id);
}