# AnointedAutomation.Logging

A small log message model with no dependencies for .NET. `LogMessage` captures the message, severity, timestamp, program name and calling method, and raises a static event every time one is created so you can route logs wherever you like.

[![NuGet](https://img.shields.io/nuget/v/AnointedAutomation.Logging.svg)](https://www.nuget.org/packages/AnointedAutomation.Logging) [![Downloads](https://img.shields.io/nuget/dt/AnointedAutomation.Logging.svg)](https://www.nuget.org/packages/AnointedAutomation.Logging)

## Installation

```bash
dotnet add package AnointedAutomation.Logging
```

- Target framework: `net10.0`
- Dependencies: none

## Quick start

```csharp
using System;
using AnointedAutomation.Optimization.Logging;

// Name your program once at startup; every LogMessage copies it into messageSource.
LogMessage.MessageSourceSetter = "MyApplication";

// Subscribe once; the event fires from every LogMessage constructor.
LogMessage.LogAdded += (sender, e) =>
{
    Console.WriteLine($"{e.log.timeStamp:O} [{e.log.messageType}] {e.log.localOperationName}: {e.log.message}");
};

LogMessage.Info("Application started.");
LogMessage.Warning(42, "Cache miss rate is high.");
LogMessage.Error("Could not reach the payment provider.");
```

Note the namespace: the types live in `AnointedAutomation.Optimization.Logging`, not `AnointedAutomation.Logging`.

## API overview

All types are in the `AnointedAutomation.Optimization.Logging` namespace.

### `LogMessage`

| Member | Description |
|---|---|
| `LogMessage()` | Empty instance (no event raised). |
| `LogMessage(MessageType messageType, string message)` | Sets `timeStamp` (`DateTime.Now`), `messageSource`, `localOperationName` and raises `LogAdded`. |
| `LogMessage(int id, MessageType messageType, string message)` | Same as above, with an id. |
| `static string MessageSourceSetter` | Program name copied into every new message's `messageSource`. |
| `static event LogAddedEventHandler LogAdded` | Raised whenever a message is constructed with a type and text. |
| `long id` | Optional identifier. |
| `string localOperationName` | Calling method, resolved from the stack trace (`Namespace.Class.Method`). Async state machines and lambdas are unwrapped to the real method name. |
| `string message` | The log text. |
| `string messageSource` | Program name (from `MessageSourceSetter`). |
| `MessageType messageType` | Severity. |
| `DateTime timeStamp` | Local creation time. |

Static factory methods, each with a `(string message)` and an `(int id, string message)` overload:

| Factory | Produces |
|---|---|
| `LogMessage.Celebrate` | `MessageType.Celebrate` |
| `LogMessage.Critical` | `MessageType.Critical` |
| `LogMessage.Error` | `MessageType.Error` |
| `LogMessage.Info` / `LogMessage.Informational` | `MessageType.Informational` |
| `LogMessage.Message` | `MessageType.Message` |
| `LogMessage.Success` | `MessageType.Success` |
| `LogMessage.Warning` | `MessageType.Warning` |

### `LogMessageEventArgs`

`EventArgs` passed to `LogAdded`; the message is exposed as the `log` property. The delegate is `LogMessage.LogAddedEventHandler(object sender, LogMessageEventArgs e)`.

### `MessageType`

`enum MessageType : int`

| Value | Number | Meaning |
|---|---|---|
| `Error` | 0 | An error that should be investigated |
| `Warning` | 1 | A potential issue |
| `Success` | 2 | A successful operation |
| `Informational` | 3 | General information |
| `Message` | 4 | A general message with no severity |
| `Critical` | 5 | A severe error needing immediate attention |
| `Celebrate` | 6 | A milestone or achievement |

## Notes

- `LogAdded` is a static event. Every `LogMessage` created anywhere in the process reaches every subscriber, so subscribe once and unsubscribe when your listener goes away.
- Creating a message captures a `StackTrace`, which is relatively expensive; avoid creating messages in very hot loops.

## Related packages

Other AnointedAutomation packages build on this one and expose their own log buffers of `LogMessage`:

- [AnointedAutomation.APIMiddlewares](https://www.nuget.org/packages/AnointedAutomation.APIMiddlewares)
- [AnointedAutomation.Repository.Mongo](https://www.nuget.org/packages/AnointedAutomation.Repository.Mongo)
- [AnointedAutomation.Repository.MySql](https://www.nuget.org/packages/AnointedAutomation.Repository.MySql)

## License

MIT. See [LICENSE](https://github.com/AnointedAutomation/AnointedAutomation/blob/master/LICENSE).

## Support This Project

This library is free and open source. The best way to support the work is to shop with us:

- **Christian items:** [https://store.anointed.company](https://store.anointed.company)
- **Everything else:** [https://www.mart.club](https://www.mart.club)

Found a bug or have a request? Open an issue at [https://github.com/AnointedAutomation/AnointedAutomation/issues](https://github.com/AnointedAutomation/AnointedAutomation/issues).
