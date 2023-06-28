namespace WonderfulRabbitsApi.Services.Interfaces;

using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models.Users;

public interface IUserService
{
    AuthenticateResponse AuthenticateUser(AuthenticateRequest model);
    List<User> GetUsers();
    Task<User> GetUser(int id);
    Task<int> RegisterUserAsync(RegisterUserModel model);
    void UpdateUser(int id, UpdateUser model);
    void DeleteUser(int id);
}