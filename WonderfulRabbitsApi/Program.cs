using WonderfulRabbitsApi.DatabaseContext;
using Microsoft.EntityFrameworkCore;
using WonderfulRabbitsApi;

var builder = WebApplication.CreateBuilder(args);

var startup = new Startup(builder.Configuration);
startup.ConfigureServices(builder.Services, builder.Environment);

var app = builder.Build();

// migrate any database changes on startup (includes initial db creation)
using (var scope = app.Services.CreateScope())
{
    var dataContext = scope.ServiceProvider.GetRequiredService<RabbitDbContext>();

    if (!dataContext.Database.IsInMemory())
    {
        dataContext.Database.Migrate();
    }
}

startup.Configure(app);

app.Run();
