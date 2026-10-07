using Microsoft.EntityFrameworkCore;
using TurnBasedWebApi;
using TurnBasedWebApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(builder.Configuration["SUPABASE_CONNECTION_STRING"]));

builder.Services.AddScoped<IMatchService, MatchService>();

var app = builder.Build();

app.UseHttpsRedirection();
app.Run();
