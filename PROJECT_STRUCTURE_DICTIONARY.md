# PROJECT STRUCTURE DICTIONARY - AnointedAutomation Platform

## Overview
This file serves as the index/directory for all PROJECT_STRUCTURE files in the AnointedAutomation .NET library solution. Use this to quickly find which file contains the information you need.

## PROJECT_STRUCTURE Files Directory

### [PROJECT_STRUCTURE_BUILD.md](./PROJECT_STRUCTURE_BUILD.md) ✅
- GitHub Actions CI/CD pipeline configuration and workflows
- Automated build, test, and deployment processes
- NuGet publishing pipeline with version detection
- Smart selective publishing and dependency resolution
- Quality gates and code coverage collection
- Environment setup and .NET 8.0 build configuration

### [PROJECT_STRUCTURE_CODE.md](./PROJECT_STRUCTURE_CODE.md) ✅
- Complete code documentation status and standards
- XML documentation coverage for all components
- Documentation style and patterns used
- Test documentation status
- Documentation verification checklist
- Examples of documentation standards

### [PROJECT_STRUCTURE_LIBRARIES.md](./PROJECT_STRUCTURE_LIBRARIES.md) ✅
- Individual library details for all 6 core .NET libraries
- Dependencies, relationships, and version information
- Public API surfaces and key classes for each library
- NuGet package specifications and MIT licensing
- Cross-library dependencies and integration patterns
- Library-specific functionality areas (Logging, API Middlewares, MongoDB Repository, etc.)

### [PROJECT_STRUCTURE_OVERVIEW.md](./PROJECT_STRUCTURE_OVERVIEW.md) ✅
- Overall .NET solution architecture and technology stack
- Main libraries organization and project structure
- Solution file organization and folder hierarchy
- Technology choices (.NET 8.0, C#, MongoDB, GitHub Actions)
- High-level component relationships
- Root directory structure and file organization

### [PROJECT_STRUCTURE_TESTING.md](./PROJECT_STRUCTURE_TESTING.md) ✅
- Test organization under "Tests" solution folder
- Xunit framework configuration for .NET 10.0
- Individual test project structures and test files (6 test projects, 314 tests)
- Test coverage patterns and testing strategies
- Test file organization by library (Logging, Memory, APIMiddlewares, Repository.Mongo, Repository.MySql, Objects.API)
- Unit test patterns and mock object usage

### [PROJECT_STRUCTURE_VISUAL.md](./PROJECT_STRUCTURE_VISUAL.md) ✅
- System architecture overview with ASCII diagrams
- Library dependency flow visualization
- NuGet publishing pipeline flowchart
- Test organization structure diagram
- Data flow architecture and component interactions
- Cross-library integration points and memory management architecture

## Quick Reference Guide

Looking for information about:
- **Build pipelines?** → Check [PROJECT_STRUCTURE_BUILD.md](./PROJECT_STRUCTURE_BUILD.md)
- **Code documentation?** → Check [PROJECT_STRUCTURE_CODE.md](./PROJECT_STRUCTURE_CODE.md)
- **Library dependencies?** → Check [PROJECT_STRUCTURE_LIBRARIES.md](./PROJECT_STRUCTURE_LIBRARIES.md)
- **Solution structure?** → Check [PROJECT_STRUCTURE_OVERVIEW.md](./PROJECT_STRUCTURE_OVERVIEW.md)
- **Test organization?** → Check [PROJECT_STRUCTURE_TESTING.md](./PROJECT_STRUCTURE_TESTING.md)
- **Visual architecture diagrams?** → Check [PROJECT_STRUCTURE_VISUAL.md](./PROJECT_STRUCTURE_VISUAL.md)
- **GitHub Actions workflows?** → Check [PROJECT_STRUCTURE_BUILD.md](./PROJECT_STRUCTURE_BUILD.md)
- **NuGet packages?** → Check [PROJECT_STRUCTURE_LIBRARIES.md](./PROJECT_STRUCTURE_LIBRARIES.md)
- **System data flow?** → Check [PROJECT_STRUCTURE_VISUAL.md](./PROJECT_STRUCTURE_VISUAL.md)

## File Size Management
Each PROJECT_STRUCTURE file is kept under 1000 lines for optimal reading performance. If a file grows too large, it will be split into more specific files.