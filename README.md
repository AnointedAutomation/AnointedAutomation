# Anointed Automation .NET Code Repository

[![GitHub](https://img.shields.io/badge/GitHub-AnointedAutomation-blue?logo=github)](https://github.com/AnointedAutomation)

Welcome to the Anointed Automation repository! This repository contains our open-sourced .NET code, aimed at providing useful libraries and tools for the community.

**GitHub Organization:** [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)

### [PROJECT_STRUCTURE.md](./PROJECT_STRUCTURE_DICTIONARY.md)

## Table of Contents

- [About Anointed Automation](#about-anointed-automation)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [NuGet Packages](#nuget-packages)
- [Contributing](#contributing)
- [License](#license)
- [Contact](#contact)

## About Anointed Automation

Anointed Automation is dedicated to delivering high-quality software solutions and fostering innovation within the .NET community. By sharing our code, we hope to empower developers and promote collaboration.

## Features

- Comprehensive libraries for various .NET applications.
- Well-documented code with examples and usage guidelines.
- Continuous integration and testing to ensure code quality.

## Installation

To get started with our libraries, you can add them to your .NET project using NuGet. Use the following command in your terminal:

```bash
dotnet add package <PackageName>
```

## NuGet Packages

Our libraries are available as NuGet packages. Here are the packages currently available:

- **AnointedAutomation.APIMiddlewares** - Middleware components for ASP.NET Core APIs
- **AnointedAutomation.Logging** - Standardized logging utilities
- **AnointedAutomation.Memory** - Memory management utilities
- **AnointedAutomation.Objects** - Common object models (pure POCOs)
- **AnointedAutomation.Objects.API** - API-specific object models
- **AnointedAutomation.Objects.Mongo** - MongoDB-specific entity classes with BSON serialization
- **AnointedAutomation.Repository.Mongo** - MongoDB repository pattern implementation
- **AnointedAutomation.Repository.MySql** - MySQL/EF Core repository pattern implementation

### Package Publishing

We use a dual-track deployment strategy:

- **Stable Releases**: Published from `master` branch with standard versions (e.g., `1.0.0`)
- **Pre-releases**: Published from `develop` branch with `-develop.BUILD` suffix (e.g., `1.0.0-develop.123`)

Packages are automatically published via GitHub Actions on merges to master (stable) or develop (pre-release) branches.

#### Manual Publishing

For manual package publishing, use one of the provided scripts:
- `publish-packages.ps1` (PowerShell - recommended)
- `publish-packages.sh` (Linux/Mac)
- `publish-packages.bat` (Windows)

See [PUBLISHING.md](PUBLISHING.md) for detailed instructions.
