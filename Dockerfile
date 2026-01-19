# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["AgroSolutions.sln", "./"]
COPY ["src/AgroSolutions.Domain/AgroSolutions.Domain.csproj", "src/AgroSolutions.Domain/"]
COPY ["src/AgroSolutions.Api/AgroSolutions.Api.csproj", "src/AgroSolutions.Api/"]
COPY ["src/AgroSolutions.Functions/AgroSolutions.Functions.csproj", "src/AgroSolutions.Functions/"]

# Restore dependencies
RUN dotnet restore "AgroSolutions.sln"

# Copy all source files
COPY . .

# Build the solution
WORKDIR "/src/src/AgroSolutions.Api"
RUN dotnet build "AgroSolutions.Api.csproj" -c Release -o /app/build

# Publish stage
FROM build AS publish
RUN dotnet publish "AgroSolutions.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
WORKDIR /app

# Create logs directory
RUN mkdir -p /app/logs

# Copy published files
COPY --from=publish /app/publish .

# Expose port
EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

# Set entry point
ENTRYPOINT ["dotnet", "AgroSolutions.Api.dll"]
