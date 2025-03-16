using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebApplication1.Models;
var builder = WebApplication.CreateBuilder(args);
builder.Services.AddDbContext<PostgresContext>();
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
            .ToArray();
        return forecast;
    })
    .WithName("GetWeatherForecast")
    .WithOpenApi();

app.MapGet("/userlist", (PostgresContext db) => db.Users.ToList());

// Сериализция нашего списка в json объект
app.MapGet("/user/{id:int}", (PostgresContext db, int id) =>JsonConvert.SerializeObject(db.Users.FirstOrDefault(p => p.Id == id),
    Formatting.Indented, new JsonSerializerSettings() {ReferenceLoopHandling = ReferenceLoopHandling.Ignore}));

// Удаление юзера
app.MapDelete("/deleteuser/{id:int}", (PostgresContext db, int id) =>
{
    db.Users.Remove(db.Users.FirstOrDefault(p => p.Id == id));
    db.SaveChanges();
});

// Обновление юзера
app.MapPut("/updateuser", (PostgresContext db, User user) =>
{
    db.Users.Update(user);
    db.SaveChanges();
} ); 

// Создание юзера
app.MapPost("/createuser", (PostgresContext db, User user) =>
{
    db.Users.Add(user);
    db.SaveChanges();
});

app.Run();

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
