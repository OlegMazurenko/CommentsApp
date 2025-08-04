# Stage 1: build
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src

# Copy all backend source files
COPY . .

# Restore dependencies
WORKDIR /src/CommentsApp.API
RUN dotnet restore

# Publish the application
RUN dotnet publish CommentsApp.API.csproj -c Release -o /app/publish

# Stage 2: runtime
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime
WORKDIR /app

# Copy the published app from build stage
COPY --from=build /app/publish .

# Copy fonts required for CAPTCHA rendering
COPY CommentsApp.API/Fonts ./Fonts

# Set the port and start command
ENV ASPNETCORE_URLS=http://+:5000
EXPOSE 5000
ENTRYPOINT ["dotnet", "CommentsApp.API.dll"]