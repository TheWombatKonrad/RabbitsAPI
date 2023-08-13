namespace WonderfulRabbitsApi.Services.Interfaces;

using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models.Users;

public interface IUserService
{
    Task<int> RegisterUserAsync(RegisterUserModel model);
    AuthenticateResponseModel AuthenticateUser(AuthenticateRequestModel model);
    Task<User> GetUserAsync(int id);
    Task<List<User>> GetUsersAsync();
    Task UpdateUserAsync(int id, UpdateUserModel model);
    Task DeleteUserAsync(int id);
}