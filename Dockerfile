# Use the official ASP.NET Core runtime as a base image
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80

# Environment variable to configure ASP.NET Core to listen on port 80
ENV ASPNETCORE_URLS=http://+:80
ENV ASPNETCORE_ENVIRONMENT=Docker

# Use the official .NET SDK as a build image
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore as distinct layers
COPY ["WeatherMicroservice.Api/WeatherMicroservice.Api.csproj", "WeatherMicroservice.Api/"]
COPY ["WeatherMicroservice.Core/WeatherMicroservice.Core.csproj", "WeatherMicroservice.Core/"]
COPY ["WeatherMicroservice.Infrastructure/WeatherMicroservice.Infrastructure.csproj", "WeatherMicroservice.Infrastructure/"]
COPY ["WeatherMicroservice.Tests/WeatherMicroservice.Tests.csproj", "WeatherMicroservice.Tests/"]

RUN dotnet restore "WeatherMicroservice.Api/WeatherMicroservice.Api.csproj"

# Copy the remaining files and build the project
COPY . .
WORKDIR "/src/WeatherMicroservice.Api"
RUN dotnet build "WeatherMicroservice.Api.csproj" -c Release -o /app/build

# Publish the project
FROM build AS publish
RUN dotnet publish "WeatherMicroservice.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Use the base image to serve the application
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "WeatherMicroservice.Api.dll"]
