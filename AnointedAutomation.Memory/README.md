# AnointedAutomation.Memory

A .NET library providing memory management utilities for optimizing application performance through controlled garbage collection.

## Overview

The AnointedAutomation.Memory library offers utilities to manage and optimize memory usage in .NET applications. It provides controlled garbage collection mechanisms to help return unused memory to the operating system.

## Installation

Install via NuGet:

```bash
dotnet add package AnointedAutomation.Memory
```

## Usage

### GarbageCollection Class

The `GarbageCollection` class provides methods to perform manual garbage collection and attempt to return unused memory to the operating system.

```csharp
using AnointedAutomation.Optimization.Memory;

// Create a new instance
var gc = new GarbageCollection();

// Perform garbage collection
gc.PerformGarbageCollection(null);
```

### Key Features

- **Manual Garbage Collection**: Triggers garbage collection on demand
- **Memory Return to OS**: Attempts to return unused memory back to the operating system
- **No-GC Region Management**: Uses .NET's no-GC region feature to optimize memory return

### How It Works

1. **Garbage Collection**: Calls `GC.Collect()` and waits for pending finalizers
2. **Memory Assessment**: Checks total memory usage
3. **No-GC Region**: Attempts to create a no-GC region to facilitate memory return
4. **Memory Release**: Ends the no-GC region, triggering memory return to the OS

## Best Practices

- Use sparingly - manual garbage collection can impact performance
- Best suited for scenarios where large amounts of memory need to be released
- Consider using after completing memory-intensive operations
- Monitor application performance when implementing manual GC

## Requirements

- .NET Standard 2.0 or higher
- Compatible with .NET Core 2.0+ and .NET Framework 4.6.1+

## License

Copyright © 2023 Anointed Automation, LLC. All rights reserved.

## Author

Created by Alexander Fields
GitHub: [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)