using Minio;
using Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// MinIO регистрируется в FileService, здесь регистрируем только сервис
builder.Services.AddScoped<FileService>();

var app = builder.Build();

app.UseAuthorization();
app.MapControllers();

app.Run();