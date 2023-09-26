using System.ComponentModel.DataAnnotations;

namespace WonderfulRabbitsApi.Entities
{
    public class Rabbit
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime? Birthdate { get; set; }
        public virtual User User { get; set; }
        // public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
    }
}