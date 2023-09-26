using Microsoft.EntityFrameworkCore;
using WonderfulRabbitsApi.DatabaseContext;

namespace WonderfulRabbitsApiTests;

public class TestDatabaseFixture
{
    private const string connectionString = @"Server=.\SQLExpress;Database=EFTestSample;Trusted_Connection=True;TrustServerCertificate=True";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;

    public TestDatabaseFixture()
    {
        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();
                }

                _databaseInitialized = true;
            }
        }
    }

    public RabbitDbContext CreateContext()
        => new RabbitDbContext(
            new DbContextOptionsBuilder<RabbitDbContext>()
                .UseSqlServer(connectionString)
                .Options);
}