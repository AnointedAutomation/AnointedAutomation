[← Back to Dictionary](./PROJECT_STRUCTURE_DICTIONARY.md)

# PROJECT STRUCTURE BUILD

## CI/CD Pipeline Overview
The AnointedAutomation solution uses GitHub Actions for automated build, test, and deployment with sophisticated version management and quality gates.

## GitHub Actions Workflows

### 1. build-and-test.yml (Continuous Integration)
**Triggers:**
- Pull requests to master and develop branches
- Push to master and develop branches

**Jobs:**
- **Build**: Compiles all projects in solution
- **Test**: Runs all test suites with code coverage
- **Coverage**: Collects XPlat Code Coverage data

**Environment:** Ubuntu-latest, .NET 8.0

### 2. nuget-publish.yml (Primary Publishing Pipeline)
**Triggers:**
- Push to master branch (production releases)
- Manual workflow dispatch

**Smart Features:**
- **Version Detection**: Only publishes packages with incremented versions
- **Selective Publishing**: Compares with NuGet.org to avoid duplicates
- **Dependency Resolution**: Uses `check-version-changes.sh` script

**Publishing Targets:**
1. **Primary**: NuGet.org (public packages)
2. **Secondary**: GitHub Packages (backup/internal)

**Required Secrets:**
- `NUGET_API_KEY` - NuGet.org publishing
- `GITHUB_TOKEN` - GitHub Packages publishing

### 3. publish-nuget.yml (Alternative Publishing)
**Triggers:**
- Manual workflow dispatch
- Release creation

**Features:**
- Simplified publishing workflow
- Direct package building and pushing
- Less sophisticated version checking

### 4. codeql-analysis.yml (Security Scanning)
**Triggers:**
- Weekly schedule (Saturdays at 5:47 UTC)
- Push to master branch
- Pull requests targeting master

**Analysis:**
- Static code analysis for security vulnerabilities
- Automated security scanning with CodeQL
- Integration with GitHub Security tab

## Manual Publishing Scripts

### publish-packages.sh
**Purpose:** Manual package publishing with comprehensive safety checks

**Features:**
- Version validation against NuGet.org
- Build verification before publishing
- Error handling and rollback capabilities
- Interactive confirmation prompts
- Comprehensive logging

**Usage:**
```bash
./publish-packages.sh
```

**Safety Checks:**
- Verifies .NET SDK installation
- Validates project build success
- Confirms version increments
- Checks NuGet API key configuration

## Build Configuration

### Solution Structure
- **Main Solution**: `AnointedAutomation.sln`
- **Projects**: 6 libraries + 4 test projects
- **Target Framework**: .NET 8.0
- **Configuration**: Debug/Release builds

### Package Configuration (NuGet.config)
```xml
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <packageSources>
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" protocolVersion="3" />
  </packageSources>
</configuration>
```

### .NET Installation
- **Script**: `dotnet-install.sh` (Official Microsoft installer)
- **Purpose**: Ensures .NET 8.0 SDK availability in CI/CD environments

## Version Management Strategy

### Semantic Versioning
- **Format**: MAJOR.MINOR.PATCH (e.g., 0.0.5)
- **Control**: `<Version>` tags in individual .csproj files
- **Current Versions**:
  - AnointedAutomation.Logging: 0.0.5
  - AnointedAutomation.Memory: 0.0.4
  - AnointedAutomation.APIMiddlewares: 0.0.13
  - AnointedAutomation.Repository.Mongo: 0.0.5
  - AnointedAutomation.Objects: 0.0.12
  - AnointedAutomation.Objects.API: 0.0.5

### Automated Version Detection
- **Script**: `check-version-changes.sh`
- **Function**: Compares local versions with published NuGet versions
- **Logic**: Only publishes packages with incremented version numbers
- **Benefits**: Prevents duplicate publishing and failed builds

## Build Commands

### Local Development
```bash
# Build solution
~/.dotnet/dotnet build

# Run tests
~/.dotnet/dotnet test

# Build specific project
~/.dotnet/dotnet build AnointedAutomation.{LibraryName}/

# Test specific project
~/.dotnet/dotnet test AnointedAutomation.{LibraryName}.Tests/
```

### Package Creation
```bash
# Create packages
~/.dotnet/dotnet pack --configuration Release

# Create specific package
~/.dotnet/dotnet pack AnointedAutomation.{LibraryName}/ --configuration Release
```

### Publishing (Manual)
```bash
# Publish to NuGet.org
~/.dotnet/dotnet nuget push "AnointedAutomation.{LibraryName}/bin/Release/*.nupkg" --api-key $NUGET_API_KEY --source https://api.nuget.org/v3/index.json

# Publish to GitHub Packages
~/.dotnet/dotnet nuget push "AnointedAutomation.{LibraryName}/bin/Release/*.nupkg" --api-key $GITHUB_TOKEN --source https://nuget.pkg.github.com/AnointedAutomation/index.json
```

## Quality Gates

### Pre-Publishing Requirements
1. **Build Success**: All projects must compile without errors
2. **Test Passing**: All unit tests must pass
3. **Version Increment**: Package version must be higher than published version
4. **Security Scan**: CodeQL analysis must not reveal critical vulnerabilities

### Automated Checks
- **Dependency Resolution**: Ensures all internal package dependencies are satisfied
- **Package Validation**: Verifies package metadata and structure
- **API Key Validation**: Confirms publishing credentials before deployment

## Environment Requirements

### Development Environment
- **.NET 8.0 SDK** (required)
- **Git** (for version control)
- **Text Editor/IDE** (Visual Studio, VS Code, etc.)

### CI/CD Environment
- **OS**: Ubuntu-latest (GitHub Actions)
- **Runtime**: .NET 8.0
- **Package Manager**: NuGet CLI
- **Secrets**: NUGET_API_KEY, GITHUB_TOKEN

## Deployment Strategy

### Stable Releases
- **Branch**: master
- **Trigger**: Direct push or merged PR
- **Destination**: NuGet.org + GitHub Packages
- **Versioning**: Semantic versioning (stable)

### Development Releases
- **Branch**: develop (not currently configured for auto-publish)
- **Manual**: Via workflow dispatch
- **Versioning**: Pre-release suffixes (if implemented)

## Troubleshooting and Documentation

### PUBLISHING.md
Comprehensive guide covering:
- Step-by-step publishing instructions
- Common error scenarios and solutions
- Version management best practices
- API key configuration
- Package validation procedures

### Monitoring and Logs
- **GitHub Actions**: Full build/test/deploy logs
- **NuGet.org**: Package download statistics
- **GitHub Packages**: Internal package metrics
- **Security**: CodeQL scan results and alerts