# -------- Base Runtime Image (.NET 9 Stable) --------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS base
USER app
WORKDIR /app
EXPOSE 8080

# -------- Build Image --------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src

# Copy everything at once (because of inter-project dependencies and shared props/targets)
COPY . .

# Restore via the .sln file (full project graph)
RUN dotnet restore "ZeroFat.sln"

# Build the main Web API project
WORKDIR "/src/src/Host/ZeroFat.WebAPIs"
RUN dotnet build "ZeroFat.WebAPIs.csproj" -c $BUILD_CONFIGURATION -o /app/build

# -------- Publish Image --------
FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "ZeroFat.WebAPIs.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

# -------- Final Runtime Container --------
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "ZeroFat.WebAPIs.dll"]
