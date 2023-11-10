using WonderfulRabbitsApi.Models.Images;
using WonderfulRabbitsApi.Models.Users;

namespace WonderfulRabbitsApi.Models.Rabbits;
public class RabbitModel
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DateTime? Birthdate { get; set; }
    public UserDataModel User { get; set; }
    public virtual ICollection<ImageModel> Images { get; set; } = new List<ImageModel>();
}