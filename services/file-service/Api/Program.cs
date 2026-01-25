using Minio;
using Microsoft.AspNetCore.HttpOverrides;
using Api;
using Api.Services;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Api.Repositories;
using System.Security.Claims;
using Npgsql.EntityFrameworkCore.PostgreSQL;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// --- 1. Настройка БД ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=postgres;Database=filesdb;Username=user;Password=pass"));

// --- 2. Настройка Порта ---
builder.WebHost.UseUrls("http://0.0.0.0:8080");

// --- 3. Настройка JWT Аутентификации (Самое важное) ---
// Мы говорим .NET: "Если видишь заголовок Authorization, проверяй его как JWT"
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    // Ключ шифрования. В продакшене он должен быть в переменных окружения!
    // Здесь мы берем его из конфига или используем безопасный фоллбэк, чтобы Docker не падал.
    var keyString = builder.Configuration["Jwt:Key"] ?? "super_secret_key_must_be_very_long_at_least_32_chars";
    var key = Encoding.UTF8.GetBytes(keyString);

    options.TokenValidationParameters = new TokenValidationParameters
    {
        // Проверять, что токен подписан именно нашим ключом
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),

        // Проверять, не истек ли срок действия (7 дней)
        ValidateLifetime = true,

        // Убираем задержку на "протухание" (по умолчанию 5 минут)
        ClockSkew = TimeSpan.Zero,

        // Пока отключаем проверку издателя (Issuer) и аудитории (Audience) для простоты Docker-сети
        ValidateIssuer = false,
        ValidateAudience = false
    };
});

// --- 4. Регистрация сервисов ---
builder.Services.AddScoped<IUserRepository, UserRepositoryDb>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddScoped<JwtTokenService>(); // Наш генератор
builder.Services.AddScoped<UserAuthorizationService>();
builder.Services.AddScoped<MinioService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Настройка Swagger, чтобы в нем можно было вводить Токен (удобно для тестов)
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Description = "Вставь токен так: Bearer {твой_токен}",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});

var app = builder.Build();

// --- 5. Middleware Pipeline (Порядок важен!) ---

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Миграции
using (var scope = app.Services.CreateScope())
{
    try
    {
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при миграции: {ex.Message}");
    }
}

// Статика (HTML/CSS)
app.UseDefaultFiles();
app.UseStaticFiles();

// --- БЛОК БЕЗОПАСНОСТИ ---
app.UseAuthentication(); // <--- ЭТОГО НЕ БЫЛО. Проверяет "Кто ты?" (читает токен)
app.UseAuthorization();  // <--- Проверяет "Можно ли тебе сюда?" (читает роль/доступ)

app.MapControllers();

app.Run();