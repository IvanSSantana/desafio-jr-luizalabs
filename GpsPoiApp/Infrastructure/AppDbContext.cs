using Microsoft.EntityFrameworkCore;
using GpsPoiApp.Models;

namespace GpsPoiApp.Infrastructure;

public class AppDbContext : DbContext
{
    public DbSet<Point> Points { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }   
}