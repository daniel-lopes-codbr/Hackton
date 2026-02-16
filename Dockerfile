# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy csproj and restore dependencies
COPY ["src/AgroSolutions.Api/AgroSolutions.Api.csproj", "src/AgroSolutions.Api/"]
COPY ["src/AgroSolutions.Application/AgroSolutions.Application.csproj", "src/AgroSolutions.Application/"]
COPY ["src/AgroSolutions.Domain/AgroSolutions.Domain.csproj", "src/AgroSolutions.Domain/"]
COPY ["src/AgroSolutions.Infrastructure/AgroSolutions.Infrastructure.csproj", "src/AgroSolutions.Infrastructure/"]
COPY ["tests/AgroSolutions.UnitTests/AgroSolutions.UnitTests.csproj", "tests/AgroSolutions.UnitTests/"]

# Restore packages
RUN dotnet restore "src/AgroSolutions.Api/AgroSolutions.Api.csproj"
RUN dotnet restore "tests/AgroSolutions.UnitTests/AgroSolutions.UnitTests.csproj"

# Copy all source files
COPY . .

# Build the projects
RUN dotnet build "src/AgroSolutions.Api/AgroSolutions.Api.csproj" -c Release
RUN dotnet build "tests/AgroSolutions.UnitTests/AgroSolutions.UnitTests.csproj" -c Release

# Test stage
FROM build AS test
WORKDIR /src
RUN dotnet test "tests/AgroSolutions.UnitTests/AgroSolutions.UnitTests.csproj" -c Release --no-build

# Build and publish
FROM build AS publish
RUN dotnet build "src/AgroSolutions.Api/AgroSolutions.Api.csproj" -c Release -o /app/build
RUN dotnet publish "src/AgroSolutions.Api/AgroSolutions.Api.csproj" -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Set environment variables
ENV ASPNETCORE_ENVIRONMENT=Development
ENV ASPNETCORE_URLS=http://+:80

# Copy the published app
COPY --from=publish /app/publish .

# Create a non-root user
RUN adduser --disabled-password --gecos "" appuser && chown -R appuser /app
USER appuser

# Expose port 80
EXPOSE 80

# Set the entry point
ENTRYPOINT ["dotnet", "AgroSolutions.Api.dll"]

