REM setup environment and then run dotnet with whatever passed params
set DATABASE_SERVICE_NAME=127.0.0.1,1401
set DB_ADMIN_PASSWORD=Test1234
set DB_USER=test
set DB_PASSWORD=Test4321
set DB_DATABASE=cllc
set ASPNETCORE_ENVIRONMENT=Development
set PathBase=/cannabislicensing

REM e.g. dotnet run, dotnet test, etc.
dotnet %*

