//https://www.c-sharpcorner.com/article/unit-testing-with-inmemory-provider-and-sqlite-in-memory-database-in-efcore/

using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using WonderfulRabbitsApi.DatabaseContext;

public class SqliteConnectionFactory : IDisposable
{
    private bool disposedValue = false; // To detect redundant calls  

    public RabbitDbContext CreateContextForSQLite()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var option = new DbContextOptionsBuilder<RabbitDbContext>()
            .UseSqlite(connection).Options;

        var context = new RabbitDbContext(option);

        if (context != null)
        {
            context.Database.EnsureDeleted();
            context.Database.EnsureCreated();
        }

        return context;
    }


    protected virtual void Dispose(bool disposing)
    {
        if (!disposedValue)
        {
            if (disposing)
            {
            }

            disposedValue = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
    }
}
