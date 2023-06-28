using Microsoft.EntityFrameworkCore;
using WonderfulRabbitsApi.Entities;

namespace WonderfulRabbitsApi.DatabaseContext;

public interface IRabbitDbContext
{
    DbSet<User>? Users { get; set; }
    DbSet<Photo>? Photos { get; set; }
    DbSet<Rabbit>? Rabbits { get; set; }
    int SaveChangesAsync();
}