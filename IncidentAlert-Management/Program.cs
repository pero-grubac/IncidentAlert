using IncidentAlert_Management.Consumers.IncidentConsumers;
using IncidentAlert_Management.Data;
using IncidentAlert_Management.JWT;
using IncidentAlert_Management.Middleware;
using IncidentAlert_Management.Models;
using IncidentAlert_Management.Models.Dto;
using IncidentAlert_Management.Repositories;
using IncidentAlert_Management.Repositories.Implementation;
using IncidentAlert_Management.Services;
using IncidentAlert_Management.Services.Implementation;
using MassTransit;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Please insert JWT with Bearer into field",
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});


// Database
builder.Services.AddDbContext<DataContext>(
    o => o.UseNpgsql(builder.Configuration.GetConnectionString("IncidentAlertManagementDB")));

// User
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 6;
    options.Password.RequiredUniqueChars = 1;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.AllowedForNewUsers = true;
    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
    options.User.RequireUniqueEmail = false;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();
builder.Services.AddSingleton(TimeProvider.System);

// CORS
var frontendUrl = builder.Configuration.GetValue<string>("FrontendUrl");
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin",
        builder =>
        {
            builder.WithOrigins(frontendUrl)
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .AllowCredentials()
                   .WithExposedHeaders("Authorization");

        });
});

// Logging
builder.Host.UseSerilog((context, loggerConfig) =>
{
    loggerConfig.ReadFrom.Configuration(context.Configuration);
});

// Security
var jwtSettingsSection = builder.Configuration.GetSection("JwtSettings");
var jwtSettings = jwtSettingsSection.Get<JwtSettings>() ?? new JwtSettings();
JwtSettings.Validate(jwtSettings, builder.Environment);
builder.Services.AddSingleton(jwtSettings);

var googleSettings = builder.Configuration.GetSection("GoogleSettings").Get<GoogleSettings>() ?? new GoogleSettings();
builder.Services.AddSingleton(googleSettings);
builder.Services.AddSingleton<IGoogleTokenValidator, GoogleTokenValidator>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{

    options.TokenValidationParameters = new TokenValidationParameters
    {

        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Secret))

    };
});


// Repository
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ILocationRepository, LocationRepository>();
builder.Services.AddScoped<IIncidentRepository, IncidentRepository>();
builder.Services.AddScoped<IIncidentCategoryRepository, IncidentCategoryRepository>();
builder.Services.AddScoped<IImageRepository, ImageRepository>();
builder.Services.AddScoped<IUserRepository, UserRepository>();


// Service
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ILocationService, LocationService>();
builder.Services.AddScoped<IIncidentService, IncidentService>();
builder.Services.AddScoped<IImageService, ImageService>();
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<ICustomPasswordService, CustomPasswordService>();

// HttpContext
builder.Services.AddHttpContextAccessor();

// MassTransit
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();

    // Consumers
    busConfigurator.AddConsumer<IncidentCreatedConsumer>();

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

// Liveness probe for Docker and the CI smoke test
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.ApplyMigrations();
    await app.SeedData();
}

// app.UseHttpsRedirection();

// Use static files
app.UseStaticFiles();

app.UseCors("AllowSpecificOrigin");

// Exception middleware (after CORS, so error responses still reach the browser)
app.UseMiddleware<ExceptionMiddleware>();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Exposed so the integration tests can host the API with WebApplicationFactory<Program>.
public partial class Program { }
