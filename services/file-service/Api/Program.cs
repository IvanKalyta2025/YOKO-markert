using Minio;
using Microsoft.AspNetCore.HttpOverrides;
using Api;
using Api.Services;
using Api.Data;
using Microsoft.EntityFrameworkCore;
using Api.Repositories;
using System.Security.Claims;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// --- Настройка БД ---
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=postgres;Database=filesdb;Username=user;Password=pass"));

// --- Настройка Порта (важно для Traefik) ---
builder.WebHost.UseUrls("http://0.0.0.0:8080");

// --- Сервисы ---
builder.Services.AddScoped<IUserRepository, UserRepositoryDb>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserAuthorizationService>();
builder.Services.AddControllers();
builder.Services.AddScoped<MinioService>();

var app = builder.Build();

// 1. Прокси заголовки (Всегда первым для Traefik)
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

// 2. Swagger (только в Dev)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// 3. Миграции
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<AppDbContext>();
        context.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Ошибка при миграции: {ex.Message}");
    }
}

// --- ВАЖНО: Статические файлы СНАЧАЛА ---

// Превращает запрос "/" в "/index.html"
app.UseDefaultFiles();

// Отдает файлы из папки wwwroot
app.UseStaticFiles();

// --- Логика API ПОТОМ ---
app.UseAuthorization();
app.MapControllers();

app.Run();