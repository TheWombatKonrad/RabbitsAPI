using System.ComponentModel.DataAnnotations;

namespace WonderfulRabbitsApi.Models.Users
{
    public class AuthenticateRequestModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
