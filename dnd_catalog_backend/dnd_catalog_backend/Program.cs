// Program.cs (ОБНОВЛЕННЫЙ КОД ДЛЯ RENDER)

using dnd_catalog_backend.Models;
using dnd_catalog_backend.Services;
using MongoDB.Driver;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;

const string ReactAppPolicy = "_reactAppPolicy";

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// 1. СЕРВИСЫ
// ----------------------------------------------------

builder.Services.Configure<DndDatabaseSettings>(
    builder.Configuration.GetSection("DndDatabaseSettings"));

builder.Services.AddSingleton<IMongoClient>(s =>
{
    var settings = builder.Configuration.GetSection("DndDatabaseSettings").Get<DndDatabaseSettings>();
    if (settings == null || settings.ConnectionString == null)
        throw new InvalidOperationException("MongoDB ConnectionString not found in configuration.");

    return new MongoClient(settings.ConnectionString);
});

builder.Services.AddSingleton<IMongoDatabase>(s =>
{
    var settings = builder.Configuration.GetSection("DndDatabaseSettings").Get<DndDatabaseSettings>();
    var client = s.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings!.DatabaseName);
});

builder.Services.AddSingleton<IDndCatalogService, DndCatalogService>();
builder.Services.AddSingleton<IAuthService, AuthService>();

// ✅ Нужно для AiController (IHttpClientFactory)
builder.Services.AddHttpClient();

// JWT
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = jwtSettings["Key"]
    ?? throw new InvalidOperationException("JwtSettings:Key не найдено. Проверь Environment Variables.");

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        ValidateIssuer = true,
        ValidIssuer = jwtSettings["Issuer"],

        ValidateAudience = true,
        ValidAudience = jwtSettings["Audience"],

        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// ----------------------------------------------------
// CORS (Render + локально)
// ----------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy(ReactAppPolicy, policy =>
    {
        policy
            .WithOrigins(
                "https://dnd-catalog-frontend.onrender.com",
                "http://localhost:3000",
                "https://localhost:3000"
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
        // ❌ УБРАЛИ AllowCredentials() — для Authorization: Bearer не нужно
    });
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(
            System.Text.Unicode.UnicodeRanges.BasicLatin,
            System.Text.Unicode.UnicodeRanges.Cyrillic,
            System.Text.Unicode.UnicodeRanges.All
        );
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------------------------------
// 2. PIPELINE
// ----------------------------------------------------
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// ✅ CORS ДО auth/authorization и ДО MapControllers
app.UseCors(ReactAppPolicy);

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
