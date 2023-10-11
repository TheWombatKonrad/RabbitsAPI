
using System.ComponentModel.DataAnnotations;

namespace WonderfulRabbitsApi.Models.Users;
public class RegisterUserModel
{
    [StringLength(16, ErrorMessage = "The {0} value cannot exceed {1} characters.")]
    public string Username { get; set; }

    [StringLength(16, MinimumLength = 8, ErrorMessage = "The {0} must be between {2} and {1} characters.")]
    public string Password { get; set; }

    [EmailAddress]
    public string Email { get; set; }
}