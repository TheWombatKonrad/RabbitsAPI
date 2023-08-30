using WonderfulRabbitsApi.Entities;
using WonderfulRabbitsApi.Models.Rabbits;

namespace WonderfulRabbitsApi.Models.Users
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public ICollection<RabbitModel> Rabbits { get; set; } = new List<RabbitModel>();
    }
}