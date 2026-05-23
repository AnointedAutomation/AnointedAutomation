[← Back to Dictionary](./PROJECT_STRUCTURE_DICTIONARY.md)

# PROJECT STRUCTURE VISUAL - AnointedAutomation Platform

## System Architecture Overview

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                        AnointedAutomation .NET Library Suite                         │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│  ┌─────────────┐  ┌──────────────┐  ┌─────────────┐  ┌────────────┐  ┌───────────┐ │
│  │   Logging   │  │ APIMiddleware│  │   Memory    │  │  MongoDB   │  │ Utilities │ │
│  │   Library   │  │   Library    │  │   Library   │  │ Repository │  │  Library  │ │
│  └─────────────┘  └──────────────┘  └─────────────┘  └────────────┘  └───────────┘ │
│         │                 │                 │              │              │        │
│         └─────────────────┼─────────────────┼──────────────┼──────────────┘        │
│                           │                 │              │                       │
│  ┌─────────────────────────────────────────────────────────────────────────────┐   │
│  │                        Base Library (Core)                                 │   │
│  │                     Common Types & Interfaces                              │   │
│  └─────────────────────────────────────────────────────────────────────────────┘   │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

## Library Dependency Flow

```
                           ┌─────────────────┐
                           │   Base Library  │
                           │  (Foundation)   │
                           └─────────┬───────┘
                                     │
              ┌──────────────────────┼──────────────────────┐
              │                      │                      │
              ▼                      ▼                      ▼
    ┌─────────────┐        ┌─────────────┐        ┌─────────────┐
    │   Logging   │        │   Memory    │        │  Utilities  │
    │             │        │             │        │             │
    └─────────────┘        └─────────────┘        └─────────────┘
              │                      │                      │
              └──────────────────────┼──────────────────────┘
                                     │
                                     ▼
                           ┌─────────────┐
                           │ MongoDB     │
                           │ Repository  │
                           └─────┬───────┘
                                 │
                                 ▼
                           ┌─────────────┐
                           │ API         │
                           │ Middleware  │
                           └─────────────┘
```

## NuGet Package Publishing Pipeline

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                        GitHub Actions CI/CD Pipeline                        │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  1. Source Code Analysis                                                    │
│     • Detect changed projects                                               │
│     • Parse version changes in .csproj files                               │
│     • Determine publishing dependencies                                     │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  2. Build & Test Phase                                                      │
│     • .NET 8.0 build for all projects                                       │
│     • MSTest execution with coverage                                        │
│     • Quality gate validation                                               │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  3. Smart Publishing Logic                                                  │
│     • Dependency-aware ordering                                             │
│     • Version conflict resolution                                           │
│     • Selective package publishing                                          │
└─────────────────────────────────────────────────────────────────────────────┘
                                    │
                                    ▼
┌─────────────────────────────────────────────────────────────────────────────┐
│  4. NuGet Publication                                                       │
│     • Authenticated NuGet.org publishing                                    │
│     • Package validation and verification                                   │
│     • Success/failure reporting                                             │
└─────────────────────────────────────────────────────────────────────────────┘
```

## Test Organization Structure

```
AnointedAutomation.sln
├── Libraries/
│   ├── Base/
│   ├── Logging/
│   ├── Memory/
│   ├── MongoDB/
│   ├── APIMiddleware/
│   └── Utilities/
└── Tests/
    ├── BaseTests/
    │   ├── BaseTypesTests.cs
    │   ├── InterfaceTests.cs
    │   └── UtilityTests.cs
    ├── LoggingTests/
    │   ├── LoggerTests.cs
    │   ├── FormatterTests.cs
    │   └── ConfigTests.cs
    ├── MemoryTests/
    │   ├── CacheTests.cs
    │   ├── PoolTests.cs
    │   └── BufferTests.cs
    ├── MongoDBTests/
    │   ├── RepositoryTests.cs
    │   ├── ConnectionTests.cs
    │   └── QueryTests.cs
    ├── APIMiddlewareTests/
    │   ├── AuthTests.cs
    │   ├── CorsTests.cs
    │   └── RateLimitTests.cs
    └── UtilitiesTests/
        ├── ExtensionTests.cs
        ├── HelperTests.cs
        └── ValidationTests.cs
```

## Data Flow Architecture

```
┌─────────────────┐    ┌─────────────────┐    ┌─────────────────┐
│   Client App    │───▶│  API Middleware │───▶│   Business      │
│                 │    │                 │    │     Logic       │
└─────────────────┘    └─────────────────┘    └─────────────────┘
                                │                        │
                                ▼                        ▼
                    ┌─────────────────┐    ┌─────────────────┐
                    │    Logging      │    │   MongoDB       │
                    │    System       │    │   Repository    │
                    └─────────────────┘    └─────────────────┘
                                │                        │
                                ▼                        ▼
                    ┌─────────────────┐    ┌─────────────────┐
                    │   Memory        │    │   Database      │
                    │   Management    │    │   Storage       │
                    └─────────────────┘    └─────────────────┘
```

## Component Interaction Flow

```
Application Startup
        │
        ▼
┌─────────────────┐
│ Dependency      │
│ Injection       │──┐
│ Container       │  │
└─────────────────┘  │
        │            │
        ▼            │
┌─────────────────┐  │    ┌─────────────────┐
│ Base Services   │  │    │ Configuration   │
│ Registration    │◀─┘    │ Validation      │
└─────────────────┘       └─────────────────┘
        │
        ▼
┌─────────────────┐
│ Middleware      │
│ Pipeline        │──┐
│ Setup           │  │
└─────────────────┘  │
        │            │
        ▼            │
┌─────────────────┐  │    ┌─────────────────┐
│ Request         │  │    │ Authentication  │
│ Processing      │◀─┘    │ & Authorization │
└─────────────────┘       └─────────────────┘
        │
        ▼
┌─────────────────┐
│ Business Logic  │──┐
│ Execution       │  │
└─────────────────┘  │
        │            │
        ▼            │
┌─────────────────┐  │    ┌─────────────────┐
│ Data Access     │  │    │ Caching &       │
│ Layer           │◀─┘    │ Memory Mgmt     │
└─────────────────┘       └─────────────────┘
        │
        ▼
┌─────────────────┐
│ Response        │──┐
│ Generation      │  │
└─────────────────┘  │
        │            │
        ▼            │
┌─────────────────┐  │    ┌─────────────────┐
│ Logging &       │  │    │ Error Handling  │
│ Monitoring      │◀─┘    │ & Cleanup       │
└─────────────────┘       └─────────────────┘
```

## Solution File Structure

```
AnointedAutomation/
├── .github/
│   └── workflows/
│       └── dotnet.yml (CI/CD Pipeline)
├── Libraries/
│   ├── AnointedAutomation.Base/
│   │   ├── AnointedAutomation.Base.csproj
│   │   └── [Core interfaces & types]
│   ├── AnointedAutomation.Logging/
│   │   ├── AnointedAutomation.Logging.csproj
│   │   └── [Logging implementation]
│   ├── AnointedAutomation.Memory/
│   │   ├── AnointedAutomation.Memory.csproj
│   │   └── [Memory management]
│   ├── AnointedAutomation.MongoDB/
│   │   ├── AnointedAutomation.MongoDB.csproj
│   │   └── [MongoDB repository]
│   ├── AnointedAutomation.APIMiddleware/
│   │   ├── AnointedAutomation.APIMiddleware.csproj
│   │   └── [API middleware]
│   └── AnointedAutomation.Utilities/
│       ├── AnointedAutomation.Utilities.csproj
│       └── [Utility functions]
├── Tests/
│   ├── AnointedAutomation.BaseTests/
│   ├── AnointedAutomation.LoggingTests/
│   ├── AnointedAutomation.MemoryTests/
│   ├── AnointedAutomation.MongoDBTests/
│   ├── AnointedAutomation.APIMiddlewareTests/
│   └── AnointedAutomation.UtilitiesTests/
├── AnointedAutomation.sln
├── LICENSE (MIT)
└── README.md
```

## GitHub Actions Workflow Visualization

```
┌─────────────────────────────────────────────────────────────────────────────┐
│                              Trigger Events                                 │
│                                                                             │
│  Push to Master/Develop  │  Pull Request  │  Manual Workflow Dispatch      │
└─────────────┬───────────────────┬─────────────────────┬───────────────────┘
              │                   │                     │
              └───────────────────┼─────────────────────┘
                                  │
                                  ▼
              ┌─────────────────────────────────────────────────────┐
              │               Setup Environment                      │
              │  • Ubuntu Latest Runner                             │
              │  • .NET 8.0 SDK Installation                        │
              │  • NuGet Authentication                             │
              └─────────────────────┬───────────────────────────────┘
                                    │
                                    ▼
              ┌─────────────────────────────────────────────────────┐
              │            Project Analysis Phase                   │
              │  • Detect changed projects via git diff            │
              │  • Parse .csproj files for version changes         │
              │  • Build dependency resolution matrix              │
              └─────────────────────┬───────────────────────────────┘
                                    │
                                    ▼
              ┌─────────────────────────────────────────────────────┐
              │               Build & Test Phase                    │
              │  • Restore NuGet packages                          │
              │  • Build all projects (.NET 8.0)                  │
              │  • Run MSTest suite with coverage                 │
              │  • Quality gate validation                         │
              └─────────────────────┬───────────────────────────────┘
                                    │
                                    ▼
              ┌─────────────────────────────────────────────────────┐
              │            Smart Publishing Logic                   │
              │  • Order packages by dependency hierarchy          │
              │  • Resolve version conflicts                       │
              │  • Generate NuGet packages for changed projects    │
              └─────────────────────┬───────────────────────────────┘
                                    │
                                    ▼
              ┌─────────────────────────────────────────────────────┐
              │             NuGet Publication                       │
              │  • Publish to NuGet.org with API key              │
              │  • Verify successful publication                   │
              │  • Report results and artifacts                    │
              └─────────────────────────────────────────────────────┘
```

## Memory Management Architecture

```
┌─────────────────────────────────────────────────────────────────┐
│                    Memory Management Layer                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                 │
│  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐            │
│  │   Object    │  │   Buffer    │  │   Cache     │            │
│  │   Pooling   │  │   Management│  │   Management│            │
│  └──────┬──────┘  └──────┬──────┘  └──────┬──────┘            │
│         │                │                │                   │
│         └────────────────┼────────────────┘                   │
│                          │                                    │
│  ┌─────────────────────────────────────────────────────────┐  │
│  │              Base Memory Interfaces                     │  │
│  │           (IMemoryPool, IBufferManager, etc.)          │  │
│  └─────────────────────────────────────────────────────────┘  │
│                                                                │
└─────────────────────────────────────────────────────────────────┘
                                 │
                                 ▼
         ┌─────────────────────────────────────────────┐
         │           Application Memory Usage           │
         │                                             │
         │  • Reduced GC pressure                      │
         │  • Improved performance                     │
         │  • Resource optimization                    │
         └─────────────────────────────────────────────┘
```

## Cross-Library Integration Points

```
                    ┌─────────────────────────────────────┐
                    │          Client Application          │
                    └─────────────┬───────────────────────┘
                                  │
                    ┌─────────────▼───────────────────────┐
                    │         API Middleware Layer         │
                    │  • Authentication                    │
                    │  • CORS handling                     │
                    │  • Rate limiting                     │
                    │  • Request/response logging          │
                    └─────────────┬───────────────────────┘
                                  │
                    ┌─────────────▼───────────────────────┐
                    │        Business Logic Layer         │
                    │  • Service implementations          │
                    │  • Domain logic                     │
                    │  • Validation                       │
                    └─────────────┬───────────────────────┘
                                  │
        ┌─────────────────────────┼─────────────────────────┐
        │                         │                         │
        ▼                         ▼                         ▼
┌─────────────┐         ┌─────────────┐         ┌─────────────┐
│   Logging   │         │   Memory    │         │  MongoDB    │
│   Service   │         │   Service   │         │ Repository  │
│             │         │             │         │             │
│ • Structured│         │ • Object    │         │ • CRUD ops  │
│   logging   │         │   pooling   │         │ • Queries   │
│ • Multiple  │         │ • Buffer    │         │ • Indexes   │
│   targets   │         │   mgmt      │         │ • Relations │
│ • Filtering │         │ • Caching   │         │ • Validation│
└─────────────┘         └─────────────┘         └─────────────┘
        │                         │                         │
        └─────────────────────────┼─────────────────────────┘
                                  │
                    ┌─────────────▼───────────────────────┐
                    │          Utilities Layer             │
                    │  • Extensions                        │
                    │  • Helpers                          │
                    │  • Validators                       │
                    │  • Converters                       │
                    └─────────────────────────────────────┘
```
---

**[← Back to Project Dictionary](./PROJECT_STRUCTURE_DICTIONARY.md)**
