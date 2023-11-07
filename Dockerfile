#Orgbook service Dockerfile
FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS base
WORKDIR /app
EXPOSE 8080
ENV ASPNETCORE_URLS=http://*:8080

FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build

RUN dotnet tool install --tool-path /tools dotnet-trace
RUN dotnet tool install --tool-path /tools dotnet-counters
RUN dotnet tool install --tool-path /tools dotnet-dump

WORKDIR /src
COPY ["orgbook-service/orgbook-service.csproj", "orgbook-service/"]
COPY ["cllc-interfaces/Dynamics-Autorest/DynamicsAutorest.csproj", "cllc-interfaces/Dynamics-Autorest/"]
RUN dotnet restore "orgbook-service/orgbook-service.csproj"
COPY . .
WORKDIR "/src/orgbook-service"
RUN dotnet build "orgbook-service.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "orgbook-service.csproj" -c Release -o /app/publish --runtime linux-musl-x64 --no-self-contained

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "orgbook-service.dll"]