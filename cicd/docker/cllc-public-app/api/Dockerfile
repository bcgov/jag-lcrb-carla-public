# Dockerfile.api (dotnet 8)
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ./cllc-public-app/*.sln ./

# copy all project files; adjust if your solution layout differs
COPY ./cllc-public-app/ ./
RUN dotnet restore
RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
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
