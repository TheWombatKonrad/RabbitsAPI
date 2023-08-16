using WonderfulRabbitsApi.Entities;

namespace WonderfulRabbitsApi.Models.Rabbits;
public class RabbitModel
{
    public string Name { get; set; }
    public DateTime? Birthdate { get; set; }
    public User User { get; set; }
    public virtual ICollection<Photo> Photos { get; set; } = new List<Photo>();
}