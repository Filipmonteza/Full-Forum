using FullForum_Infrastructure;
using FullForum_Infrastructure.Persistence;
using FullForum_Infrastructure.Seeding;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registrera Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

// Om du använder controllers
builder.Services.AddControllers();

var app = builder.Build();

// Kör migration + seeding vid uppstart
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var dbContext = services.GetRequiredService<ForumDbContext>();

    await dbContext.Database.MigrateAsync();
    await DatabaseSeeder.SeedAsync(services);
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Om du använder auth/identity senare
app.UseAuthentication();
app.UseAuthorization();

// Om du använder controllers
app.MapControllers();

// Du kan ta bort weatherforecast om du inte behöver den
app.Run();