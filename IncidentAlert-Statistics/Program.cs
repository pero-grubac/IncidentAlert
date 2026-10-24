using IncidentAlert_Statistics.Service;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using IncidentAlert_Statistics.Service.Implementation;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// IncidentAlert service
var incidentAlertBaseUrl = builder.Configuration.GetValue<string>("IncidentAlertService:BaseUrl");
builder.Services.AddHttpClient("IncidentAlert", client =>
{
    client.BaseAddress = new Uri(incidentAlertBaseUrl!);
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
                   .AllowAnyHeader()
                   .AllowCredentials()
;
        });
});

// Service
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
// Only signed-in moderators may read the analysis. Tokens are issued by the Management service,
// so this service must be configured with the same JwtSettings (issuer, audience and secret).
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var jwtSecret = jwtSettings["Secret"];
if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
    throw new InvalidOperationException("JwtSettings:Secret must be set to the Management service's secret (JwtSettings__Secret / JWT_SECRET).");
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings["Issuer"],
        ValidAudience = jwtSettings["Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret)),
    });
builder.Services.AddAuthorization();

// Liveness probe for Docker and the CI smoke test
builder.Services.AddHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();

// Exposed so the integration tests can host the API with WebApplicationFactory<Program>.
public partial class Program { }
