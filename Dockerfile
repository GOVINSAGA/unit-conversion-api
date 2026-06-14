FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy solution and project files first (layer caching)
COPY UnitConversionApi.sln* ./
COPY src/UnitConversionApi/UnitConversionApi.csproj src/UnitConversionApi/
RUN dotnet restore

# Copy everything and build
COPY . .
RUN dotnet publish src/UnitConversionApi/UnitConversionApi.csproj -c Release -o /app/publish --no-restore

# Runtime image
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Non-root user for security
USER $APP_UID

EXPOSE 8080
ENV ASPNETCORE_URLS=http://+:8080

ENTRYPOINT ["dotnet", "UnitConversionApi.dll"]
