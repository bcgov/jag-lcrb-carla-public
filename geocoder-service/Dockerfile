FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["geocoder-service.csproj", ""]
RUN dotnet restore "./geocoder-service.csproj"
COPY . .
WORKDIR "/src/."
RUN dotnet build "geocoder-service.csproj" -c Release -o /app

FROM build AS publish
RUN dotnet publish "geocoder-service.csproj" -c Release -o /app

FROM base AS final
WORKDIR /app
COPY --from=publish /app .
ENTRYPOINT ["dotnet", "geocoder-service.dll"]