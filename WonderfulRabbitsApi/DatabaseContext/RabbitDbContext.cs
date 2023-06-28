using WonderfulRabbitsApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace WonderfulRabbitsApi.DatabaseContext
{
    public class RabbitDbContext : DbContext
    {
        protected readonly IConfiguration Configuration;

        public RabbitDbContext(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            // connect to sql server database
            options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));

            //related data is loaded from the database when navigation property is accessed
            //meaning helps with the relationships in entities
            options.UseLazyLoadingProxies();
        }

        public virtual DbSet<User>? Users { get; set; }
        public virtual DbSet<Photo>? Photos { get; set; }
        public virtual DbSet<Rabbit>? Rabbits { get; set; }

    }
}
