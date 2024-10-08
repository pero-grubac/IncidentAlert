using IncidentAlert_ML.Service;
using IncidentAlert_ML.Service.Implementation;

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
}).ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true
});



// service
builder.Services.AddScoped<IIncidentGroupingService, IncidentGroupingService>();

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

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowSpecificOrigin");

app.UseAuthorization();

app.MapControllers();

app.Run();
