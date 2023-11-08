namespace WonderfulRabbitsApi.Models.Rabbits;

using System.ComponentModel.DataAnnotations;
using WonderfulRabbitsApi.Entities;

public class RegisterRabbitModel
{
    [StringLength(16, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
    public string Name { get; set; }
    public DateTime? Birthdate { get; set; }
    public int? UserId { get; set; }
    public virtual ICollection<Image>? Images { get; set; } = new List<Image>();
}