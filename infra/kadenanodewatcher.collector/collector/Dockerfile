#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.

FROM mcr.microsoft.com/dotnet/runtime:8.0 AS base
USER app
WORKDIR /app

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["KadenaNodeWatcher.Collector/KadenaNodeWatcher.Collector.csproj", "KadenaNodeWatcher.Collector/"]
COPY ["KadenaNodeWatcher.Core/KadenaNodeWatcher.Core.csproj", "KadenaNodeWatcher.Core/"]
COPY ["DbConnectionExtensions/DbConnectionExtensions.csproj", "DbConnectionExtensions/"]
RUN dotnet restore "./KadenaNodeWatcher.Collector/KadenaNodeWatcher.Collector.csproj"
COPY . .
WORKDIR "/src/KadenaNodeWatcher.Collector"
RUN dotnet build "./KadenaNodeWatcher.Collector.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./KadenaNodeWatcher.Collector.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app/collector
COPY --from=publish /app/publish .

# Create the db and logs directories within the container
RUN mkdir //app/data
RUN mkdir //app/data/db
RUN mkdir //app/infra
RUN mkdir //app/infra/logs

ENTRYPOINT ["dotnet", "KadenaNodeWatcher.Collector.dll"]
CMD ["-a", "-g", "-n 100"]