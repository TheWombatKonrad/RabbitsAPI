
using System.ComponentModel.DataAnnotations;

namespace WonderfulRabbitsApi.Models.Users;
public class RegisterUserModel
{
    [StringLength(16, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
    [Required]
    public string Username { get; set; }

    [StringLength(16, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
    [Required]
    public string Password { get; set; }

    [EmailAddress]
    [Required]
    public string Email { get; set; }
}