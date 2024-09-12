using IncidentAlert.Data;
using IncidentAlert.Models;
using IncidentAlert.Models.Dto;
using IncidentAlert.Repositories;
using IncidentAlert.Repositories.Implementation;
using IncidentAlert.Services;
using IncidentAlert.Services.Implementation;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Model mapper
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Database
builder.Services.AddDbContext<DataContext>(
    o => o.UseNpgsql(builder.Configuration.GetConnectionString("IncidentAlertDB")));

// Repository
builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<>));

// Service
builder.Services.AddScoped<IService<CategoryDto, Category>, CategoryService>();


// Logging
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
