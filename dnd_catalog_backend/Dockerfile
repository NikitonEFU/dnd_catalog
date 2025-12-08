# 1. СТАДИЯ СБОРКИ (Используем .NET SDK для компиляции)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Копируем файл проекта и восстанавливаем зависимости
# Это ускоряет сборку, если только зависимости не менялись
COPY ["dnd_catalog_backend.csproj", "."]
RUN dotnet restore

# Копируем остальной код проекта
COPY . .

# Публикуем приложение
RUN dotnet publish -c Release -o /app/publish

# 2. СТАДИЯ ЗАПУСКА (Используем легкий образ ASP.NET для запуска)
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Копируем опубликованные файлы с предыдущего этапа
COPY --from=build /app/publish .

# Запускаем приложение
# Убедитесь, что 'dnd_catalog_backend.dll' совпадает с именем вашего основного DLL
ENTRYPOINT ["dotnet", "dnd_catalog_backend.dll"]
