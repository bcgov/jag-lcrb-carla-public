# Code Quality and Formatting

This project uses multiple tools for code quality and formatting:

- [CSharpier](https://csharpier.com/) for C# code formatting
- [ESLint](https://eslint.org/) for TypeScript/JavaScript linting
- [Prettier](https://prettier.io/) for frontend code formatting

## Backend (C#) - CSharpier

### Setup

The tool is configured as a local .NET tool. To install it:

```bash
# Restore all .NET tools (including CSharpier)
dotnet tool restore
```

### Manual Usage

#### Check formatting (without making changes)

```bash
# Check all C# files for formatting issues
dotnet csharpier --check .

# Check specific files or directories
dotnet csharpier --check ./Controllers
dotnet csharpier --check ./Controllers/ApplicationsController.cs
```

#### Fix formatting

```bash
# Format all C# files
dotnet csharpier .

# Format specific files or directories
dotnet csharpier ./Controllers
dotnet csharpier ./Controllers/ApplicationsController.cs
```

#### Other useful commands

```bash
# Check version
dotnet csharpier --version

# Show help
dotnet csharpier --help

# Dry run (show what would be changed without making changes)
dotnet csharpier --check --verbose .
```

### Configuration

- **Configuration file**: `.csharpierrc.yml` - Contains formatting rules and preferences
- **Ignore file**: `.csharpierignore` - Specifies files and folders to exclude from formatting
- **Tool manifest**: `.config/dotnet-tools.json` - Defines the CSharpier tool version

## Frontend (TypeScript/JavaScript) - ESLint & Prettier

### Setup

Install dependencies in the ClientApp directory:

```bash
cd cllc-public-app/ClientApp
npm install
```

### Manual Usage

#### ESLint (Linting)

```bash
# Check for linting issues
npm run lint

# Fix linting issues automatically
npm run lint-fix
```

#### Prettier (Formatting)

```bash
# Check for formatting issues
npm run format

# Fix formatting issues automatically
npm run format-fix
```

### Configuration

- **ESLint**: `.eslintrc.*` files in ClientApp directory
- **Prettier**: `.prettierrc*` files in ClientApp directory
- **Package scripts**: Defined in `cllc-public-app/ClientApp/package.json`

## GitHub Actions

Two GitHub Actions workflows automatically run on pull requests:

### 1. Backend Code Formatting (`.github/workflows/code-quality-backend.yml`)

- Runs on C# file changes
- Checks CSharpier formatting
- Posts PR comments if issues are found

### 2. Frontend Code Quality (`.github/workflows/code-quality-frontend.yml`)

- Runs on frontend file changes
- Checks ESLint and Prettier
- Posts PR comments with fix instructions

## IDE Integration

### Visual Studio Code

**Backend (C#):**

- Install the [CSharpier extension](https://marketplace.visualstudio.com/items?itemName=csharpier.csharpier-vscode)

**Frontend:**

- Install [ESLint extension](https://marketplace.visualstudio.com/items?itemName=dbaeumer.vscode-eslint)
- Install [Prettier extension](https://marketplace.visualstudio.com/items?itemName=esbenp.prettier-vscode)

### Visual Studio

CSharpier can be configured as an external tool or used via the command line.

## Quick Fix Commands

If CI checks fail, run these commands locally:

### Backend Issues

```bash
# Install tools and fix formatting
dotnet tool restore
dotnet csharpier .
```

### Frontend Issues

```bash
# Navigate to ClientApp and fix issues
cd cllc-public-app/ClientApp
npm install
npm run lint-fix
npm run format-fix
```

## Notes

- Tools are **not** integrated into the build process to avoid breaking existing builds
- They're designed to be run manually or as part of CI/CD checks
- Backend configuration excludes generated files, build outputs, and client-side code
- Frontend checks cover TypeScript, JavaScript, HTML, CSS, and SCSS files
