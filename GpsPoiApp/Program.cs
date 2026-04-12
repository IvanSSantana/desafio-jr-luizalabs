using GpsPoiApp.Infrastructure;
using GpsPoiApp.Infrastructure.Middlewares;
using GpsPoiApp.Repository;
using GpsPoiApp.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

#region Dependency Injection
builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<IPointService, PointService>();
builder.Services.AddScoped<IPointRepository, PointRepository>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseMiddleware<NotFoundMiddleware>();
app.UseMiddleware<InternalErrorMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

#region Database Migration and Seeding

if (!app.Environment.IsEnvironment("Testing"))
{
    using (var scope = app.Services.CreateScope())
    {
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>(); 

        dbContext.Database.Migrate();
        DbSeeding.Seed(dbContext);
    }
}

#endregion
 
app.Run();
