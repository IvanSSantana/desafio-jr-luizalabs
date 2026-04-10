using GpsPoiApp.Infrastructure;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace GpsPoiApp.Helper;

public static class TestDbFactory
{
    public static (AppDbContext context, SqliteConnection connection) CreateInMemoryDbContext()
    {
        var connection = new SqliteConnection("DataSource=:memory:");
        connection.Open();

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseSqlite(connection)
            .Options;

        var context = new AppDbContext(options);

        context.Database.EnsureDeleted(); // Ensures that it doesn't exist an pre-existing database
        context.Database.EnsureCreated(); // Grants the creation of the database schema according to DbContext of real database 
        DbSeeding.Seed(context); 

        return (context, connection);
    }
}