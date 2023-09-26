//https://learn.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-7.0

using System.Data.Common;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.EntityFrameworkCore;
using WonderfulRabbitsApi.DatabaseContext;

namespace WonderfulRabbitsApiTests;

public class CustomWebApplicationFactory<TProgram>
    : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(services =>
        {
            var dbContextDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbContextOptions<RabbitDbContext>));

            services.Remove(dbContextDescriptor);

            var dbConnectionDescriptor = services.SingleOrDefault(
                d => d.ServiceType ==
                    typeof(DbConnection));

            services.Remove(dbConnectionDescriptor);

            services.AddDbContext<RabbitDbContext>((container, options) =>
            {
                options.UseInMemoryDatabase("InMemoryDbForTesting");
            });

            //services.AddSingleton<ISeedDataClass, SeedDataClass>();

            var sp = services.BuildServiceProvider();

            using (var scope = sp.CreateScope())
            {
                var scopedServices = scope.ServiceProvider;
                var appDb = scopedServices.GetRequiredService<RabbitDbContext>();

                //var seeder = scopedServices.GetRequiredService<ISeedDataClass>();

                appDb.Database.EnsureDeleted();
                appDb.Database.EnsureCreated();

                // seeder.InitializeDbForTests(); //TODO: put this in trycatch with logger
            }

        });

        builder.UseEnvironment("Test");
    }
}