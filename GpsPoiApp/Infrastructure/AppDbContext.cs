using Microsoft.EntityFrameworkCore;
using GpsPoiApp.Models;

namespace GpsPoiApp.Infrastructure;

public class AppDbContext : DbContext
{   
    // Allowing dependency injection of DbContextOptions for testing 
    public AppDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Point> Points { get; set; }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=app.db");
    }   

    // Mapping the Point model to the database schema
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Point>(entity =>
        {
            entity.HasKey(p => p.Id);
            
            entity.Property(p => p.Name)
                .HasMaxLength(100);

            entity.Property(p => p.X).IsRequired();
            entity.Property(p => p.Y).IsRequired();
        });
    }
}