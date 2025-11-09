using CareSync.ApplicationLayer;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.DataLayer;
using CareSync.DataLayer.Entities;
using CareSync.InfrastructureLayer.Services.EntitiesServices;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseSerilog();

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContextPool<CareSyncDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CareSyncConnection")));
builder.Services.AddIdentity<T_Users, T_Roles>().AddEntityFrameworkStores<CareSyncDbContext>().AddDefaultTokenProviders();


builder.Services.AddAutoMapper(typeof(ApplicationLayerDependencies).Assembly);
#region Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File($"Logs/logs{DateTime.Today.Date}.txt", rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddSerilog();
#endregion

#region Services
builder.Services.AddScoped<IUserService, UserService>();
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.ApplicationLayerDI();
app.Run();
