using System.ComponentModel.DataAnnotations;

namespace WonderfulRabbitsApi.Models.Users
{
    public class AuthenticateRequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}
