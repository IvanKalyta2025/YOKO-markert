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
// var appswagger = builder.Build();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql("Host=postgres;Database=filesdb;Username=user;Password=pass"));

builder.WebHost.UseUrls("http://0.0.0.0:8080");

builder.Services.AddScoped<IUserRepository, UserRepositoryDb>();
builder.Services.AddScoped<IProfileRepository, ProfileRepository>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<UserService>();
builder.Services.AddControllers();
builder.Services.AddScoped<FileService>();

var app = builder.Build();
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

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
        // Если база еще не готова, здесь можно логировать ошибку
        Console.WriteLine($"Ошибка при миграции: {ex.Message}");
    }
}


app.UseAuthorization();
app.MapControllers();

app.Run();



// builder.Services.AddDbContext<AppDbContext>(options =>
//     options.UseNpgsql("Host=postgres;Database=filesdb;Username=user;Password=pass"));