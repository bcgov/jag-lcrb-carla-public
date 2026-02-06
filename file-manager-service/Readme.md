# File Manager Service

The purpose of the file manager service is to act as an interface to SharePoint.

# gRPC

The file manager service exposes a gRPC API.

See `Services/FileManagerService.cs`.

# HTTP

The file manager service exposes an HTTP API.

See `Controllers/FileManagerController.cs`.

Important: The HTTP API only exists as a wrapper around the gRPC API, which is the source of truth. This was added to make it easier for external systems to talk to the file manager service, if they didn't or couldn't make native gRPC calls.
