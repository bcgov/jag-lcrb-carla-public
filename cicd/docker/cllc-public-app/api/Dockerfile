# Dockerfile.api (dotnet 6.0)
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build
WORKDIR /src

# Copy project files
COPY ./cllc-interfaces/ ./cllc-interfaces/
COPY ./cllc-public-app/ ./cllc-public-app/
COPY ./file-manager-service/ ./file-manager-service/

# Restore dependencies
RUN dotnet restore ./cllc-public-app/cllc-public-app.sln

# Build and publish the main project
RUN dotnet publish ./cllc-public-app/cllc-public-app.csproj -c Release -o /app/publish /p:UseAppHost=false

FROM mcr.microsoft.com/dotnet/aspnet:6.0 AS runtime
WORKDIR /app

# OpenShift best-practice: listen on 8080 and run as non-root (random UID)
ENV ASPNETCORE_URLS=http://+:8080
ENV DOTNET_RUNNING_IN_CONTAINER=true
COPY --from=build /app/publish ./

# Add simple health endpoints if you use Microsoft.Extensions.Diagnostics.HealthChecks
# Otherwise, update probes in the Helm chart to match an existing endpoint.
EXPOSE 8080

# No USER line: allows OpenShift to inject a random, non-root UID
ENTRYPOINT ["dotnet","cllc-public-app.dll"]
