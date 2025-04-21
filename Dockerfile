# Use the official ASP.NET Core image for runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

# Use the SDK image for building the application
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy the project files
COPY ["BookyWeb/BookyWeb.csproj", "BookyWeb/"]
COPY ["Booky.DataAccess/Booky.DataAccess.csproj", "Booky.DataAccess/"]
COPY ["Booky.Models/Booky.Models.csproj", "Booky.Models/"]
COPY ["Booky.Utility/Booky.Utility.csproj", "Booky.Utility/"]

# Restore dependencies
RUN dotnet restore "BookyWeb/BookyWeb.csproj"

# Copy the rest of the source
COPY . .

WORKDIR "/src/BookyWeb"
RUN dotnet build "BookyWeb.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "BookyWeb.csproj" -c Release -o /app/publish

# Final runtime image
FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .

ENTRYPOINT ["dotnet", "BookyWeb.dll"]
