using IncidentAlert.Consumers.CategoryConsumers;
using IncidentAlert.Consumers.IncidentConsumers;
using IncidentAlert.Data;
using IncidentAlert.Middleware;
using IncidentAlert.Repositories;
using IncidentAlert.Repositories.Implementation;
using IncidentAlert.Services;
using IncidentAlert.Services.Implementation;
using MassTransit;
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
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<IIncidentCategoryRepository, IncidentCategoryRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();

// Service
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IImageService, ImageService>();

// Logging
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
});

// CORS
var frontendUrl = builder.Configuration.GetValue<string>("FrontendUrl");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins(frontendUrl)
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

// HttpContext
builder.Services.AddHttpContextAccessor();

// MassTransit
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();

    // Consumer
    busConfigurator.AddConsumer<CategoryCreatedConsumer>();
    busConfigurator.AddConsumer<CategoryDeletedConsumer>();
    busConfigurator.AddConsumer<CategoryUpdatedConsumer>();

    busConfigurator.AddConsumer<IncidentApprovedConsumer>();

    busConfigurator.UsingRabbitMq((context, configurator) =>
    {
        configurator.Host(new Uri(builder.Configuration["MessageBroker:Host"]!), h =>
        {
            h.Username(builder.Configuration["MessageBroker:Username"]!);
            h.Password(builder.Configuration["MessageBroker:Password"]!);
        });

        configurator.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
}
// Exception middleware
app.UseMiddleware<ExceptionMiddleware>();

app.UseHttpsRedirection();
// Use static files
app.UseStaticFiles();

//app.UseAuthorization();
app.UseCors("AllowSpecificOrigin");

app.MapControllers();

app.Run();
