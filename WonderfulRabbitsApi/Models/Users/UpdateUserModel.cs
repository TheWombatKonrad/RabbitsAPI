using System.ComponentModel.DataAnnotations;

namespace WonderfulRabbitsApi.Models.Users;

public class UpdateUserModel
{
    public string? Username { get; set; }
    public string? Password { get; set; }
    [EmailAddress]
    public string? Email { get; set; }
}