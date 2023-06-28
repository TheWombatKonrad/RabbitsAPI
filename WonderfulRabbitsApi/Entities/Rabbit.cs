using System.ComponentModel.DataAnnotations;

namespace WonderfulRabbitsApi.Entities
{
    public class Rabbit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime Birthdate { get; set; }
        public virtual ICollection<Photo> Photos {get; set;} = new List<Photo>();
        public virtual ICollection<User> Users { get; set; } = new List<User>();
    }
}