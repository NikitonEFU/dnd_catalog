# 1. СТАДИЯ СБОРКИ (Используем .NET SDK для компиляции)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# ВАЖНОЕ ИЗМЕНЕНИЕ: 
# Используем команду с wildcard (*.csproj), чтобы скопировать файл
# В контексте монорепозитория это часто работает лучше.

# Копируем .csproj и восстанавливаем зависимости
COPY *.csproj . 
RUN dotnet restore

# Копируем остальной код и публикуем приложение
COPY . .
RUN dotnet publish -c Release -o /app/publish

# 2. СТАДИЯ ЗАПУСКА (Используем легкий образ ASP.NET для запуска)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Копируем опубликованные файлы с предыдущего этапа
COPY --from=build /app/publish .

# Запускаем приложение
ENTRYPOINT ["dotnet", "dnd_catalog_backend.dll"]
