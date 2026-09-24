# AnointedAutomation.Memory

A small .NET utility for on-demand garbage collection. It forces a full collection, waits for finalizers, and then tries to hand unused memory back to the operating system, which is useful for long running services after a memory heavy job.

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Memory.svg)](https://www.nuget.org/packages/AnointedAutomation.Memory) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.Memory.svg)](https://www.nuget.org/packages/AnointedAutomation.Memory)

## Installation

```bash
dotnet add package AnointedAutomation.Memory
```

- Target framework: `net10.0`
- Dependencies: none

Note that the namespace is `AnointedAutomation.Optimization.Memory`, not the package id.

## Quick start

```csharp
using AnointedAutomation.Optimization.Memory;

GarbageCollection gc = new GarbageCollection();

// After a large import, export, or batch job:
gc.PerformGarbageCollection(null);
```

## API overview

Namespace: `AnointedAutomation.Optimization.Memory`

| Member | Description |
|---|---|
| `GarbageCollection()` | Creates an instance. The class holds no state. |
| `void PerformGarbageCollection(object state)` | Runs `GC.Collect()`, then `GC.WaitForPendingFinalizers()`, then attempts to return unused memory to the OS. `state` is unused. |

How the memory return works:

1. Reads the current managed heap size with `GC.GetTotalMemory(false)`.
2. If it is greater than zero, calls `GC.TryStartNoGCRegion` with that size.
3. If the region starts, immediately calls `GC.EndNoGCRegion()`.
4. An `InvalidOperationException` from this step is swallowed, so the call never fails on it.

## Running on a timer

`PerformGarbageCollection` takes an `object state` parameter so it matches the `TimerCallback` delegate and can be scheduled directly:

```csharp
using System;
using System.Threading;
using AnointedAutomation.Optimization.Memory;

GarbageCollection gc = new GarbageCollection();
Timer timer = new Timer(gc.PerformGarbageCollection, null, TimeSpan.FromMinutes(5), TimeSpan.FromMinutes(30));
```

Keep a reference to the `Timer` for as long as it should run.

## Best practices

- Use sparingly. A forced full collection pauses the application and usually is not needed; the .NET GC tunes itself well.
- Best suited to the moment right after a large, short lived allocation spike.
- Measure before and after (for example with `GC.GetTotalMemory` or `dotnet-counters`) to confirm it helps your workload.

## Related packages

- [AnointedAutomation.APIMiddlewares](https://www.nuget.org/packages/AnointedAutomation.APIMiddlewares): ASP.NET Core middlewares that depend on this package.

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).
Copyright © Anointed Automation, LLC. Stewarded by Alexander Fields.

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue: [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues)
