using Airport.Api.Database;
using Airport.Api.Repositories;
using Airport.Api.Services;

using Microsoft.EntityFrameworkCore;


AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AirportDbContext>(options =>
{
    options.UseNpgsql(
        builder.Configuration["ConnectionStrings:DefaultConnection"],
        npgsqlOptions =>
        {
            npgsqlOptions.UseNetTopologySuite();
        }
    );
});


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddScoped<HubInsertService>();
builder.Services.AddScoped<HubRepository>();
builder.Services.AddScoped<TerminalInsertService>();
builder.Services.AddScoped<TerminalRepository>();
builder.Services.AddScoped<GateRepository>();
builder.Services.AddScoped<GateService>();
builder.Services.AddScoped<GateEventRepository>();
builder.Services.AddScoped<GateEventService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if(app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.MapControllers();

app.Run();


