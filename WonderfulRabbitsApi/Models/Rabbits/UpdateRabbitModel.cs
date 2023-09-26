
using System.ComponentModel.DataAnnotations;
using WonderfulRabbitsApi.Entities;

namespace WonderfulRabbitsApi.Models.Rabbits;

public class UpdateRabbitModel
{
    [StringLength(16, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
    public string? Name { get; set; }
    public DateTime? Birthdate { get; set; }
}