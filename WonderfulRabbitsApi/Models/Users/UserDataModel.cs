using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models.Rabbits;

namespace WonderfulRabbitsApi.Models.Users
{
    public class UserDataModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
    }
}