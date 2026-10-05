using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using Notes.Data;
using Notes.Entities;
using Notes.Repositories.Implementations;
using Notes.Repositories.Interfaces;
using Notes.Services.Implementations;
using Notes.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
ConfigurationManager configuration = builder.Configuration;


// Configuration des dépendances
builder.Services.AddScoped<INoteService, NoteService>();
builder.Services.AddScoped<INoteRepository, NoteRepository>();

builder.Services.AddControllers();


// Configuration de MongoDb
builder.Services.Configure<MongoDbSettings>(
    builder.Configuration.GetSection("MongoDb"));

builder.Services.AddSingleton<MongoDbSettings>(sp =>
    sp.GetRequiredService<Microsoft.Extensions.Options.IOptions<MongoDbSettings>>().Value);

var mongoSettings = builder.Configuration.GetSection("MongoDb").Get<MongoDbSettings>();

// Surcharge éventuelle de la ConnectionString via variable d'environnement
var envConnectionString = Environment.GetEnvironmentVariable("Mongo__ConnectionString");
if (!string.IsNullOrWhiteSpace(envConnectionString))
{
    mongoSettings.ConnectionString = envConnectionString;
}

// Utilisation des settings pour créer le client et la database
var client = new MongoClient(mongoSettings.ConnectionString);
var database = client.GetDatabase(mongoSettings.DatabaseName);
var notesCollection = database.GetCollection<Note>(mongoSettings.NotesCollectionName);

// Appel du seeder
MongoSeeder.Seed(notesCollection);


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
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
