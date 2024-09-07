using AEBackendProject.AutoMappers;
using AEBackendProject.Data;
using AEBackendProject.Models;
using AEBackendProject.Repositories;
using AEBackendProject.Services;
using Microsoft.EntityFrameworkCore;
using System;
using System.Text.Json.Serialization;
using System.Text.Json;
using AEBackendProject.Middleware;
using Microsoft.Extensions.Configuration;
using Serilog;
using AEBackendProject.Common;

var builder = WebApplication.CreateBuilder(args);

// Log
Log.Logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .WriteTo.File("logs/AEBackendLog.txt")
    .CreateLogger();
builder.Host.UseSerilog(); // Use Serilog for logging

// Add services to the container.

builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<PortSeeder>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IShipService, ShipService>();
builder.Services.AddScoped<IResponseHelper, ResponseHelper>();

builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);

var app = builder.Build();

app.UseMiddleware<ExceptionMiddleware>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Seed Port data to database
using (var serviceScope = app.Services.CreateScope())
{
    var seeder = serviceScope.ServiceProvider.GetRequiredService<PortSeeder>();
    seeder.SeedPorts();
}

app.Run();
