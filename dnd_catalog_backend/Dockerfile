# 1. СТАДИЯ СБОРКИ (Используем .NET SDK для компиляции)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# КЛЮЧЕВОЕ ИЗМЕНЕНИЕ 1: Копируем .csproj из вложенной папки
COPY dnd_catalog_backend/dnd_catalog_backend.csproj .
RUN dotnet restore

# КЛЮЧЕВОЕ ИЗМЕНЕНИЕ 2: Копируем остальной код из вложенной папки
COPY dnd_catalog_backend/ .
RUN dotnet publish -c Release -o /app/publish

# 2. СТАДИЯ ЗАПУСКА (Используем легкий образ ASP.NET для запуска)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app
COPY --from=build /app/publish .

# Запускаем приложение
ENTRYPOINT ["dotnet", "dnd_catalog_backend.dll"]
