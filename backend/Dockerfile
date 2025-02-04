# Базовый образ для сборки (включает .NET SDK)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
ARG DB_CONNECTION_STRING
ARG ASPNETCORE_ENVIRONMENT

# EF Core CLI глобально
RUN dotnet tool install --global dotnet-ef

# Добавляем путь к установленным инструментам в переменную PATH
ENV PATH="${PATH}:/root/.dotnet/tools"

WORKDIR /src

COPY Libraries/Markdown.Core /app/Libraries/Markdown.Core
COPY Libraries/Markdown.Implementation /app/Libraries/Markdown.Implementation

COPY ["MD.WebAPI/MD.WebAPI.csproj", "MD.WebAPI/"]
COPY ["MD.Application/MD.Application.csproj", "MD.Application/"]
COPY ["MD.Domain/MD.Domain.csproj", "MD.Domain/"]
COPY ["MD.Infrastructure/MD.Infrastructure.csproj", "MD.Infrastructure/"]

RUN dotnet restore "MD.WebAPI/MD.WebAPI.csproj"

COPY . .

WORKDIR "/src/MD.WebAPI"
RUN dotnet build "MD.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
RUN dotnet publish "MD.WebAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM build AS migrations
WORKDIR "/src/MD.Infrastructure"

ENV ASPNETCORE_ENVIRONMENT=$ASPNETCORE_ENVIRONMENT
ENV ConnectionStrings__PostgresConnection=$DB_CONNECTION_STRING

RUN dotnet ef migrations bundle -o /app/migrations-bundle \
    --configuration $BUILD_CONFIGURATION \
    --project ../MD.Infrastructure/MD.Infrastructure.csproj \
    --startup-project ../MD.WebAPI/MD.WebAPI.csproj

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Копируем опубликованное приложение
COPY --from=publish /app/publish .

# Копируем миграции из этапа migrations
COPY --from=migrations /app/migrations-bundle ./migrations-bundle

# Делаем миграции исполняемыми
RUN chmod +x ./migrations-bundle

# Запускаем миграции и приложение
ENTRYPOINT ["sh", "-c", "./migrations-bundle --connection \"$DB_CONNECTION_STRING\" && dotnet MD.WebAPI.dll"]