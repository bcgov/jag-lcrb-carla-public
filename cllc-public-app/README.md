## Run and Debug Portal App Locally

### Prerequisites
- .NET Core 6 installed
- npm 14 installed (via nvm) eg: 14.21.3

### Build and Run Portal App (ClientApp)
Start node app from  the `cllc-public-app/ClientApp` directory in Command Prompt
```bash
cd ClientApp
npm install
npm start
```

### Run API from Command Line (need VPN running)
If you only want to run the Browser Portal App, this is all you need.

#### Save Secrets
- Find the Secrets ID from .csproj file
- Save `secrets.json` to `%APPDATA%\Microsoft\UserSecrets\<user_secrets_id>\secrets.json`

#### Build and start the API
Open Command prompt in the this directory (`cllc-public-app`)
```bash
set ASPNETCORE_ENVIRONMENT=Development
dotnet build (this can take a while)
dotnet run
browse:
 http://localhost:5000/lcrb
 http://localhost:5000/lcrb/login/token/<Username> (eg: TestUser)
```

### Run API From VSCode (need VPN running)
You need this to edit & debug the backend API

#### Open this directory (only) in VSCode (cllc-public-app)
```bash
cd cllc-public-app
code .
```

#### VSCode Extensions needed
- .NET Code User Secrets extension (optional but useful)
- .NET Core Tools (required)

To Manage user secrets:<br>
right click on `cllc-public-app.csproj` -> "Manage User Secrets"


#### Start API from Run Menu
- Run -> Start Debugging
- Run -> Run Withoug Debugging (if no API debugging needed)
- Note: May need to specify C# and .sln file the first time you do this

#### Start API from Solution Explorer
- In VSCode Solution Explorer  (bottom left)
- Right Click on cllc-public-app -> Debug -> New intance
- This can take  a while to build the app


####  Running API
```
Hosting environment: Development
Content root path: D:\Dev\jag-lcrb-carla-fork\cllc-public-app
Now listening on: https://localhost:5001
Application started. Press Ctrl+C to shut down.
```

#### Open App in Browser
Cntl-click on the link above or open in link in browser
