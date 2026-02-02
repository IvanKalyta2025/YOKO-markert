# Используем SDK 10.0 для сборки
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

COPY *.sln .
COPY Api/*.csproj ./Api/
COPY Api.Tests/*.csproj ./Api.Tests/

RUN dotnet restore

COPY . .


# Собираем основной проект
RUN dotnet publish Api/Api.csproj --configuration Release --output /app/publish

FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS base
WORKDIR /app

COPY --from=build /app/publish .

ENTRYPOINT ["dotnet", "Api.dll"]

