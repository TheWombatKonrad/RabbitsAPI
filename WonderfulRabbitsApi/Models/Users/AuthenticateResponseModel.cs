namespace WonderfulRabbitsApi.Models.Users
{
    public class AuthenticateResponseModel
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Token { get; set; }
    }
}
