// Program.cs (ПОВНИЙ ТА ВИПРАВЛЕНИЙ КОД)

using dnd_catalog_backend.Models;
using dnd_catalog_backend.Services;
using MongoDB.Driver;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;

// Визначаємо політику CORS. Назвемо її, наприклад, "ReactAppPolicy"
const string ReactAppPolicy = "_reactAppPolicy";

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// 1. НАЛАШТУВАННЯ СЕРВІСІВ (DEPENDENCY INJECTION CONTAINER)
// ----------------------------------------------------

// 1.1. Реєстрація налаштувань DndDatabaseSettings
builder.Services.Configure<DndDatabaseSettings>(
    builder.Configuration.GetSection("DndDatabaseSettings"));

// 1.2. Реєстрація MongoDB Client та Database
builder.Services.AddSingleton<IMongoClient>(s =>
{
    var settings = builder.Configuration.GetSection("DndDatabaseSettings").Get<DndDatabaseSettings>();
    if (settings == null || settings.ConnectionString == null)
    {
        throw new InvalidOperationException("MongoDB ConnectionString not found in configuration.");
    }
    return new MongoClient(settings.ConnectionString);
});

// 1.3. Реєстрація IMongoDatabase (для зручності)
builder.Services.AddSingleton<IMongoDatabase>(s =>
{
    var settings = builder.Configuration.GetSection("DndDatabaseSettings").Get<DndDatabaseSettings>();
    var client = s.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings!.DatabaseName);
});

// 1.4. Реєстрація DndCatalogService (нашого репозиторію)
builder.Services.AddSingleton<IDndCatalogService, DndCatalogService>();

// ----------------------------------------------------
// 1.4а. Реєстрація нового сервісу автентифікації (AuthService)
// ----------------------------------------------------
builder.Services.AddSingleton<IAuthService, AuthService>();


// ----------------------------------------------------
// 1.4б. Налаштування автентифікації (JWT Bearer)
// ----------------------------------------------------
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

// Захист від помилки "Value cannot be null" (перевіряємо наявність Key)
var secretKey = jwtSettings["Key"]
    ?? throw new InvalidOperationException("Помилка конфігурації: JwtSettings:Key не знайдено у конфігурації. Перевірте appsettings.json.");

var key = Encoding.ASCII.GetBytes(secretKey);

builder.Services.AddAuthentication(options =>
{
    // Тепер JwtBearerDefaults доступний завдяки using Microsoft.AspNetCore.Authentication.JwtBearer;
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
        ClockSkew = TimeSpan.Zero // Прибираємо зміщення часу
    };
});
// ----------------------------------------------------


// 1.5. Налаштування CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: ReactAppPolicy,
        policy =>
        {
            policy.AllowAnyOrigin()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
        });
});

// 1.6. Додавання контролерів та КОНФІГУРАЦІЯ JSON-СЕРІАЛІЗАЦІЇ (ВИПРАВЛЕННЯ КОДУВАННЯ)
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Явно дозволяємо кирилицю та інші символи, щоб уникнути екранування (\uXXXX)
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(
            System.Text.Unicode.UnicodeRanges.BasicLatin,
            System.Text.Unicode.UnicodeRanges.Cyrillic,
            System.Text.Unicode.UnicodeRanges.All // Забезпечує підтримку всіх символів
        );
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------------------------------
// 2. КОНФІГУРАЦІЯ HTTP REQUEST PIPELINE
// ----------------------------------------------------
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Використовуємо CORS
app.UseCors(ReactAppPolicy);

// !!! ВАЖЛИВО: АВТЕНТИФІКАЦІЯ ПОВИННА БУТИ ПЕРЕД АВТОРИЗАЦІЄЮ !!!
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();