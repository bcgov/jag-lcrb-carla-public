# setup environment and then run dotnet with whatever passed params\

export DATABASE_SERVICE_NAME=127.0.0.1,1401
export DB_ADMIN_PASSWORD=Test1234
export DB_USER=test
export DB_PASSWORD=Test4321
export DB_DATABASE=cllc
export ASPNETCORE_ENVIRONMENT=Development
export PathBase=/cannabislicensing
export BASE_PATH=/cannabislicensing
export REDIS_SERVER=localhost:6379
export DYNAMICS_ODATA_URI=https://lclbcannabisdev.crm3.dynamics.com/api/data/v8.2/
export DYNAMICS_AAD_TENANT_ID=bcgovtrial.onmicrosoft.com
export DYNAMICS_SERVER_APP_ID_URI=https://lclbcannabisdev.crm3.dynamics.com
export DYNAMICS_CLIENT_KEY=kuZfUGu27ORFZD1I+bze6WeCL2LBFZs3tVTlsV/gjm0=
export DYNAMICS_CLIENT_ID=88ebc67f-a23e-4baa-9bf7-094605207eae
export SMTP_HOST=localhost
export ENCRYPTION_KEY=1G2UBsXdwhFJ01oQXH1beRrt6QUxaqsq

# e.g. dotnet run, dotnet test, etc.
dotnet run

