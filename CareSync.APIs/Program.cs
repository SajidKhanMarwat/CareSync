using CareSync.DataLayer;
using CareSync.DataLayer.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddDbContextPool<CareSyncDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("CareSyncConnection")));
builder.Services.AddIdentity<T_Users, T_Roles>().AddEntityFrameworkStores<CareSyncDbContext>().AddDefaultTokenProviders();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
