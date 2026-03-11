# AnointedAutomation.Logging

## Overview

The **AnointedAutomation.Logging** library provides a robust and flexible logging framework for .NET applications. It features a standardized `LogMessage` class that allows developers to create and manage detailed log entries efficiently. With support for various log message types and event-driven logging, this library is designed to streamline application monitoring and debugging.

---

## Features

- **Standardized Log Object**: Encapsulates log details like message type, timestamp, and source.
- **Event-Driven Logging**: Triggers events when new log messages are added.
- **Predefined Log Levels**: Includes types such as `Informational`, `Warning`, `Error`, `Critical`, `Success`, and more.
- **Static Factory Methods**: Simplifies log creation with utility methods.
- **Source Tracking**: Automatically captures the source of the log within the application.

---

## Installation

Include the library in your .NET project and reference the `AnointedAutomation.Logging` namespace.

---

## Usage

### Import the Namespace

```csharp
using AnointedAutomation.Logging;
```

---

### LogMessage Class

The `LogMessage` class serves as the core object for capturing log details. It supports multiple constructors and static factory methods for creating various types of log entries.

#### Properties

- **`id`**: Unique identifier for the log message.
- **`localOperationName`**: Captures the name of the method generating the log.
- **`message`**: The content of the log entry.
- **`messageSource`**: A customizable identifier for the program or module generating the log.
- **`messageType`**: Categorizes the log (e.g., `Informational`, `Error`).
- **`timeStamp`**: Timestamp when the log was created.

---

### Creating Logs

#### Using Constructors

```csharp
LogMessage log = new LogMessage(MessageType.Informational, "This is an informational message.");
```

#### Using Static Methods

```csharp
LogMessage infoLog = LogMessage.Informational("This is an informational message.");
LogMessage errorLog = LogMessage.Error("An error occurred while processing the request.");
```

#### Example with IDs

```csharp
LogMessage successLog = LogMessage.Success(1, "Operation completed successfully.");
LogMessage warningLog = LogMessage.Warning(2, "This is a warning message.");
```

---

### Events

#### LogAdded Event

Triggered whenever a new log message is created.

```csharp
LogMessage.LogAdded += (sender, args) =>
{
    Console.WriteLine($"Log Added: {args.log.message}");
};
```

---

### Log Types

The `LogMessage` class supports several predefined log types:

| Type             | Description                                 |
|------------------|---------------------------------------------|
| `Celebrate`      | Logs celebrating a milestone or achievement. |
| `Critical`       | Logs critical issues that require immediate attention. |
| `Error`          | Logs errors encountered during execution.   |
| `Informational`  | Logs general information about operations.  |
| `Message`        | Logs a general-purpose message.             |
| `Success`        | Logs successful operations or results.      |
| `Warning`        | Logs warnings that may require attention.   |

---

### Example Usage

#### Basic Logging

```csharp
LogMessage.LogAdded += (sender, args) =>
{
    Console.WriteLine($"Log: {args.log.timeStamp} - {args.log.messageType} - {args.log.message}");
};

LogMessage log = LogMessage.Error("A critical error occurred.");
```

#### Customizing the Message Source

```csharp
LogMessage.MessageSourceSetter = "MyApplication";
LogMessage log = LogMessage.Success("Application started successfully.");
```

---

## Requirements

- .NET Framework or .NET Core

---

## License

This project is copyrighted © 2023 Anointed Automation, LLC.  
**All Rights Reserved**.

---

## Author

Created by **Alexander Fields**
For inquiries, please contact [Anointed Automation](https://anointedautomation.com).
GitHub: [https://github.com/AnointedAutomation](https://github.com/AnointedAutomation)
