# Local Dev Debugging

## Version: 1.0.0

Please check that the version of `launch.json` is kept in sync with this readme.

Please update these instructions as needed, if changes are made to `launch.json`.

## Settings

Some of the more relevant settings, that may have a direct impact on this launch config working:

### Dotnet settings

- ANGULAR_DEV_SERVER = http://localhost:4200/lcrb
- ASPNETCORE_ENVIRONMENT = Development
- BASE_PATH = /lcrb

### Angular settings

- tsconfig.json -> "baseUrl": "./src"

### Other

- Any setting that involves a `path` is potentially relevant, as the debugger needs to know how to correctly find your source files and map them to the served files in the running applications.

## Start

_Note: This `launch.json` does not run the API or APP. It only attaches to the already running processes._

### 1. Run the dotnet api in a terminal

```bash
dotnet run watch
```

### 2. Run the angular app in a terminal

```bash
npm start
```

### 3. Run the debug command

From the VSCode debug panel, select the `Debug: Dotnet & Angular` command, and run it.

It should attach to the existing dotnet and angular processes, and launch a chrome window.

## Troubleshooting

### Breakpoints aren't working

If breakpoints in the backend and/or frontend are `grey`, this means that vscode was unable to correctly match the real source files to the routes being served in the browser. This likely means that one of the path settings is mismatched. This could be in `launch.json`, or `tsconfig.json`, or anywhere else that sets paths or base paths.

### VSCode is throwing errors when starting the debugger

If `launch.json` throws errors about being unable to launch a browser, or unable to attach, this is likely a permission error. Ensure that all software you are running (vscode, terminal, etc) is running using the same permission level.

> Example: On windows, if your terminal is running "as administrator" then all other software also needs to be running "as administrator". It may be an easier solution to just not run anything "as administrator".
