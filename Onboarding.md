# Developer Onboarding Guide

This guide will help you set up your development environment for the BC Liquor and Cannabis Regulation Branch application.

## Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 6 SDK](https://dotnet.microsoft.com/download/dotnet/6.0)
- [Node.js](https://nodejs.org/) (Latest LTS version recommended)
- [Visual Studio 2022](https://visualstudio.microsoft.com/vs/) or [VS Code](https://code.visualstudio.com/)
- [Git](https://git-scm.com/)
- JAG VPN access (required for MS Dynamics connectivity)

## Development Environment Setup

### 1. Clone the Repository

```bash
git clone https://github.com/bcgov/jag-lcrb-carla-public.git
cd jag-lcrb-carla-public
```

### 2. Configure User Secrets

The API requires various secrets for connecting to services like MS Dynamics and SharePoint. These secrets can be found in the Confluence page at [Insert Confluence Link].

To set up your secrets in Visual Studio:
1. In Solution Explorer, right-click the `cllc-public-app` project
2. Select "Manage User Secrets"
3. Copy the secrets from Confluence into the secrets.json file that opens

Alternatively, from the command line:
1. Navigate to the `cllc-public-app` directory
2. Run `dotnet user-secrets init`
3. Copy the secrets from Confluence and set them using:
   ```bash
   dotnet user-secrets set "KeyName" "KeyValue"
   ```

### 3. Set Up the Frontend

1. Navigate to the ClientApp directory:
   ```bash
   cd cllc-public-app/ClientApp
   ```

2. Install dependencies:
   ```bash
   npm install
   ```

### 4. Running the Application

You have two options for running the application:

#### Option 1: Using Visual Studio

1. Open the solution file in Visual Studio 2022
2. Set the startup project to `cllc-public-app`
3. Press F5 or click the "Run" button

#### Option 2: Using Command Line

1. Start the API:
   ```bash
   cd cllc-public-app
   dotnet watch
   ```

2. In a new terminal, start the Angular development server:
   ```bash
   cd cllc-public-app/ClientApp
   npm run start
   ```

The application will be available at:
- API: https://localhost:5001
- Frontend: http://localhost:4200

## Development Environment Variables

For development purposes, set:
```bash
ASPNETCORE_ENVIRONMENT=Development
```

## Additional Resources

- [Angular CLI Documentation](https://angular.io/cli)
- [.NET 6 Documentation](https://docs.microsoft.com/en-us/aspnet/core/?view=aspnetcore-6.0)
- [Project Repository](https://github.com/bcgov/jag-lcrb-carla-public)