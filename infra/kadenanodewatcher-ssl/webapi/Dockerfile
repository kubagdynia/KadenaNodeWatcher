#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
USER app
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["KadenaNodeWatcher.Api/KadenaNodeWatcher.Api.csproj", "KadenaNodeWatcher.Api/"]
COPY ["KadenaNodeWatcher.Core/KadenaNodeWatcher.Core.csproj", "KadenaNodeWatcher.Core/"]
COPY ["DbConnectionExtensions/DbConnectionExtensions.csproj", "DbConnectionExtensions/"]
RUN dotnet restore "./KadenaNodeWatcher.Api/KadenaNodeWatcher.Api.csproj"
COPY . .
WORKDIR "/src/KadenaNodeWatcher.Api"
RUN dotnet build "./KadenaNodeWatcher.Api.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KadenaNodeWatcher.Api.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app/api
COPY --from=publish /app/publish .

# Create the db and logs directories within the container
RUN mkdir //app/data
RUN mkdir //app/data/db
RUN mkdir //app/infra
RUN mkdir //app/infra/logs

ENTRYPOINT ["dotnet", "KadenaNodeWatcher.Api.dll", "--launch-profile Prod"]