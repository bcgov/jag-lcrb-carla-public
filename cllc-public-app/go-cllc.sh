#/bin/bash
# setup environment and then run dotnet with whatever passed params
export DATABASE_SERVICE_NAME=127.0.0.1,1401
export DB_ADMIN_PASSWORD=Test1234
export DB_USER=test
export DB_PASSWORD=Test4321
export DB_DATABASE=cllc
export ASPNETCORE_ENVIRONMENT=Development
export PathBase=/cannabislicensing

# e.g. dotnet run, dotnet test, etc.
dotnet "$@" --urls http://*:5000

