using Microsoft.EntityFrameworkCore;
using WonderfulRabbitsApi.DatabaseContext;

namespace WonderfulRabbitsApiTests;

public class TestDatabaseFixture
{
    private const string ConnectionString = @"Server=(localdb)\mssqllocaldb;Database=EFTestSample;Trusted_Connection=True";

    private static readonly object _lock = new();
    private static bool _databaseInitialized;
    private TestDataHelper helper;

    public TestDatabaseFixture()
    {
        helper = new TestDataHelper();

        lock (_lock)
        {
            if (!_databaseInitialized)
            {
                using (var context = CreateContext())
                {
                    context.Database.EnsureDeleted();
                    context.Database.EnsureCreated();

                    context.AddRange(helper.GetUsers());
                    context.SaveChanges();
                }

                _databaseInitialized = true;
            }
        }
    }

    public RabbitDbContext CreateContext()
        => new RabbitDbContext(
            new DbContextOptionsBuilder<RabbitDbContext>()
                .UseSqlServer(ConnectionString)
                .Options);
}