using WonderfulRabbitsApi.Entities;

namespace WonderfulRabbitsApi.Models.Users
{
    public class UserModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public virtual ICollection<Rabbit> Rabbits { get; set; } = new List<Rabbit>();
    }
}