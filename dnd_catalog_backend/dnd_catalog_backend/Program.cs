// Program.cs (ОБНОВЛЕННЫЙ КОД ДЛЯ RENDER)

using dnd_catalog_backend.Models;
using dnd_catalog_backend.Services;
using MongoDB.Driver;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authentication;

// Определяем политику CORS.
const string ReactAppPolicy = "_reactAppPolicy";

var builder = WebApplication.CreateBuilder(args);

// ----------------------------------------------------
// 1. НАСТРОЙКА СЕРВИСОВ (DEPENDENCY INJECTION CONTAINER)
// ----------------------------------------------------

// 1.1. Регистрация настроек DndDatabaseSettings
builder.Services.Configure<DndDatabaseSettings>(
    builder.Configuration.GetSection("DndDatabaseSettings"));

// 1.2. Регистрация MongoDB Client
builder.Services.AddSingleton<IMongoClient>(s =>
{
    var settings = builder.Configuration.GetSection("DndDatabaseSettings").Get<DndDatabaseSettings>();
    if (settings == null || settings.ConnectionString == null)
    {
        throw new InvalidOperationException("MongoDB ConnectionString not found in configuration.");
    }
    return new MongoClient(settings.ConnectionString);
});

// 1.3. Регистрация IMongoDatabase
builder.Services.AddSingleton<IMongoDatabase>(s =>
{
    var settings = builder.Configuration.GetSection("DndDatabaseSettings").Get<DndDatabaseSettings>();
    var client = s.GetRequiredService<IMongoClient>();
    return client.GetDatabase(settings!.DatabaseName);
});

// 1.4. Регистрация DndCatalogService (репозитория)
builder.Services.AddSingleton<IDndCatalogService, DndCatalogService>();

// 1.4а. Регистрация нового сервиса аутентификации (AuthService)
builder.Services.AddSingleton<IAuthService, AuthService>();


// 1.4б. Настройка аутентификации (JWT Bearer)
var jwtSettings = builder.Configuration.GetSection("JwtSettings");

var secretKey = jwtSettings["Key"]
    ?? throw new InvalidOperationException("Ошибка конфигурации: JwtSettings:Key не найдено в конфигурации. Проверьте Environment Variables.");

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
// 1.5. Настройка CORS для Render
// ----------------------------------------------------
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: ReactAppPolicy,
        policy =>
        {
            // !!! ИЗМЕНЕНО: Явно разрешаем домен фронтенда Render
            policy.WithOrigins("https://dnd-catalog-frontend.onrender.com")
                   .AllowAnyHeader()
                   .AllowAnyMethod()
                   .AllowCredentials(); // Разрешаем передачу учетных данных (JWT/куки)
        });
});
// ----------------------------------------------------


// 1.6. Добавление контроллеров и КОНФИГУРАЦИЯ JSON-СЕРИАЛИЗАЦИИ
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // Явно разрешаем кириллицу и другие символы
        options.JsonSerializerOptions.Encoder = System.Text.Encodings.Web.JavaScriptEncoder.Create(
            System.Text.Unicode.UnicodeRanges.BasicLatin,
            System.Text.Unicode.UnicodeRanges.Cyrillic,
            System.Text.Unicode.UnicodeRanges.All
        );
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ----------------------------------------------------
// 2. КОНФИГУРАЦИЯ HTTP REQUEST PIPELINE
// ----------------------------------------------------
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Используем CORS (должно быть до UseAuthorization)
app.UseCors(ReactAppPolicy);

// !!! ВАЖНО: АУТЕНТИФИКАЦИЯ ДОЛЖНА БЫТЬ ПЕРЕД АВТОРИЗАЦИЕЙ !!!
app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
