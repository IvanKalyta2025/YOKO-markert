# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# 1. КОПИРУЕМ ТОЛЬКО ФАЙЛЫ ПРОЕКТА/РЕШЕНИЯ ДЛЯ КЭШИРОВАНИЯ
# Assuming FileService.sln and Api.csproj are at the same level as Dockerfile
COPY *.sln .
COPY Api/*.csproj ./Api/

# 2. ВОССТАНОВЛЕНИЕ ПАКЕТОВ
RUN dotnet restore

# 3. КОПИРУЕМ ОСТАЛЬНОЙ КОД
COPY . .

# 4. СБОРКА И ПУБЛИКАЦИЯ
# Публикуем по имени папки (Api)
RUN dotnet publish Api --configuration Release --output /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0
WORKDIR /app
# Копируем результаты публикации
COPY --from=build /app/publish .
# Убедитесь, что имя DLL соответствует имени проекта (Api)
ENTRYPOINT ["dotnet", "Api.dll"]