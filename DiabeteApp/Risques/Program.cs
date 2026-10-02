using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Risques.Services;
using Risques.Services.Implementations;
using Risques.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;
var urlGateway = "http://gateway:8080"; // URL du Gateway


// Configuration des dépendances
builder.Services.AddScoped<IRisqueService, RisqueService>();
builder.Services.AddScoped<IDateProvider, SystemDateProvider>();

builder.Services.AddControllers();


// HttpContextAccessor (pour accéder à la session dans les services)
builder.Services.AddHttpContextAccessor();


// HttpClient pour appeler le Gateway
builder.Services.AddHttpClient<IPatientApiService, PatientApiService>(client =>
{
    client.BaseAddress = new Uri(urlGateway);
});

builder.Services.AddHttpClient<INoteApiService, NoteApiService>(client =>
{
    client.BaseAddress = new Uri(urlGateway);
});


// Configuration de JWT
string? key = configuration["Jwt:Key"]
           ?? Environment.GetEnvironmentVariable("JWT_SECRET_KEY");

if (string.IsNullOrEmpty(key))
{
    throw new ArgumentNullException("Jwt:Key", "La clé JWT de configuration est manquante.");
}

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();


// Construction de l'API
var app = builder.Build();


// Pipeline HTTP
//app.UseHttpsRedirection(); // Désactivé pour Docker

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
