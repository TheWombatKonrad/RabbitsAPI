using System.ComponentModel.DataAnnotations;

namespace WonderfulRabbitsApi.Entities
{
    public class User
    {
        public int Id { get; set; }

        [StringLength(16, ErrorMessage = "The {0} value cannot exceed {1} characters. ")]
        public string Username { get; set; }
        public string PasswordHash { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public virtual ICollection<Rabbit> Rabbits { get; set; } = new List<Rabbit>();
    }
}