using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TurnBasedWebApi;
using TurnBasedWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration["SUPABASE_CONNECTION_STRING"]));

builder.Services.AddScoped<IMatchService, MatchService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
});

var app = builder.Build();

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
