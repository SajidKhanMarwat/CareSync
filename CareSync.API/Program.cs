using CareSync.ApplicationLayer.Extensions;
using CareSync.ApplicationLayer.IServices.EntitiesServices;
using CareSync.ApplicationLayer.Repository;
using CareSync.ApplicationLayer.Services.EntitiesServices;
using CareSync.ApplicationLayer.UnitOfWork;
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
CareSync.InfrastructureLayer.Common.Constants.Configurations.Initialize(builder.Configuration);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContextPool<CareSyncDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CareSyncConnection")));
builder.Services.AddIdentity<T_Users, T_Roles>().AddEntityFrameworkStores<CareSyncDbContext>().AddDefaultTokenProviders();
builder.Services.AddAutoMapper(typeof(ApplicationLayerDependencies).Assembly);
builder.Services.ApplicationLayerServices();

#region Serilog
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .WriteTo.File("Logs/logs-.txt",rollingInterval: RollingInterval.Day)
    .WriteTo.Console()
    .CreateLogger();

builder.Services.AddSerilog();
#endregion

#region Services
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IAppointmentService, AppointmentService>();
builder.Services.AddScoped<IPatientService, PatientService>();
builder.Services.AddScoped<IDoctorService, DoctorService>();
builder.Services.AddScoped<ILabService, LabService>();
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

// Seed database with initial data
await app.SeedDatabaseAsync();

app.Run();
