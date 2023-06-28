using WonderfulRabbitsApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace WonderfulRabbitsApi.DatabaseContext
{
    public class RabbitDbContext : DbContext
    {
        private DbContextOptions<RabbitDbContext> _options;

        public RabbitDbContext(DbContextOptions<RabbitDbContext> options) : base(options)
        {
            _options = options;
        }

        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<Photo>? Photos { get; set; }
        public virtual DbSet<Rabbit>? Rabbits { get; set; }

    }
}
