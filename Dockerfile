# Use the official .NET SDK to build the project
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /app

# Copy everything and restore dependencies
COPY . .
RUN dotnet restore

# Build and publish in Release mode
RUN dotnet publish -c Release -o out

# Use a smaller runtime image for the final container
FROM mcr.microsoft.com/dotnet/aspnet:8.0
WORKDIR /app
COPY --from=build /app/out .

# Set the entry point to run your backend
ENTRYPOINT ["dotnet", "POSBackend.dll"]
