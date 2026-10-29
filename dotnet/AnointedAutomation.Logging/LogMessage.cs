// Copyright © Anointed Automation Corporation., 2024. Open source, provided "as is" without warranty (see LICENSE). Stewarded by Alexander Fields https://www.alexanderfields.me on 2024-06-12 11:23:55 Jesus is King ✝️
// Edited by Alexander Fields https://www.alexanderfields.me 2025-07-02 11:48:25
// Edited 2026-10-28: UTC timestamps, caller file:line, exception overloads, isolated LogAdded subscribers, long ids.
//Stewarded by Alexander Fields
using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace AnointedAutomation.Optimization.Logging
{
    /// <summary>
    /// Log Message Object for easier logging
    /// </summary>
    public class LogMessage
    {
        /// <summary>
        /// Extracts the actual method name from async state machine types
        /// </summary>
        /// <param name="stackTrace">The stack trace to analyze</param>
        /// <returns>The formatted method name</returns>
        private static string GetMethodNameFromStackTrace(StackTrace stackTrace)
        {
            // Walk up the stack to find the first non-framework method
            for (int i = 1; i < stackTrace.FrameCount; i++)
            {
                StackFrame frame = stackTrace.GetFrame(i);
                if (frame == null) continue;

                System.Reflection.MethodBase method = frame.GetMethod();
                if (method == null) continue;

                string declaringTypeName = method.DeclaringType?.FullName ?? "Unknown";
                string methodName = method.Name;

                // Skip LogMessage constructors
                if (method.DeclaringType?.Name == "LogMessage") continue;

                // Check if this is an async state machine generated method
                if (methodName == "MoveNext" && declaringTypeName.Contains("+<") && declaringTypeName.Contains(">d"))
                {
                    // Extract the actual method name from the compiler-generated type name
                    // Format: Namespace.Class+<MethodName>d__XX
                    int startIndex = declaringTypeName.IndexOf("+<") + 2;
                    int endIndex = declaringTypeName.IndexOf(">d", startIndex);
                    
                    if (startIndex > 1 && endIndex > startIndex)
                    {
                        string actualMethodName = declaringTypeName.Substring(startIndex, endIndex - startIndex);
                        // Get the actual class name (before the +)
                        string actualClassName = declaringTypeName.Substring(0, declaringTypeName.IndexOf("+<"));
                        return $"{actualClassName}.{actualMethodName}";
                    }
                }

                // Check if this is a lambda or anonymous method  
                if (methodName.Contains("<") && methodName.Contains(">"))
                {
                    // This is a lambda/anonymous method, keep looking up the stack
                    continue;
                }

                // For regular methods, return the full name
                return $"{declaringTypeName}.{methodName}";
            }

            // Fallback to the first frame method
            StackFrame fallbackFrame = stackTrace.GetFrame(1);
            if (fallbackFrame != null)
            {
                System.Reflection.MethodBase method = fallbackFrame.GetMethod();
                if (method != null)
                {
                    return $"{method.DeclaringType?.FullName ?? "Unknown"}.{method.Name}";
                }
            }

            return "Unknown";
        }

        /// <summary>
        /// Default constructor for object initializers and deserializers.
        /// IMPORTANT: this constructor does NOT stamp a time, source or operation and does NOT raise
        /// <see cref="LogAdded"/>, so a message built this way never reaches any subscriber (log store, alerting).
        /// Use a factory (<see cref="Error(string, string, string, int)"/> and friends) or a typed constructor to emit a log.
        /// </summary>
        public LogMessage()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class with a message type and content,
        /// resolves the calling method from the stack and raises <see cref="LogAdded"/>.
        /// Prefer the static factories: they record the exact caller file and line at no runtime cost.
        /// </summary>
        /// <param name="messageType">The type of log message.</param>
        /// <param name="message">The log message content.</param>
        public LogMessage(MessageType messageType, string message)
        {
            timeStamp = DateTime.UtcNow;
            messageSource = MessageSourceSetter;
            localOperationName = GetMethodNameFromStackTrace(new StackTrace());
            this.messageType = messageType;
            this.message = message;
            RaiseLogAdded(this);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessage"/> class with an ID, message type and content,
        /// resolves the calling method from the stack and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="id">The identifier for the log message.</param>
        /// <param name="messageType">The type of log message.</param>
        /// <param name="message">The log message content.</param>
        public LogMessage(long id, MessageType messageType, string message)
        {
            this.id = id;
            timeStamp = DateTime.UtcNow;
            messageSource = MessageSourceSetter;
            localOperationName = GetMethodNameFromStackTrace(new StackTrace());
            this.messageType = messageType;
            this.message = message;
            RaiseLogAdded(this);
        }

        /// <summary>
        /// Factory constructor: uses the compiler-supplied caller information instead of a stack walk.
        /// </summary>
        private LogMessage(long id, MessageType messageType, string message, Exception exception, string callerMemberName, string callerFilePath, int callerLineNumber)
        {
            this.id = id;
            timeStamp = DateTime.UtcNow;
            messageSource = MessageSourceSetter;
            this.callerMemberName = callerMemberName;
            this.callerFilePath = callerFilePath;
            this.callerLineNumber = callerLineNumber;
            localOperationName = FormatOperationName(callerMemberName, callerFilePath, callerLineNumber);
            if (localOperationName == null)
            {
                localOperationName = GetMethodNameFromStackTrace(new StackTrace());
            }
            this.messageType = messageType;
            if (exception == null)
            {
                this.message = message;
            }
            else
            {
                // The full ToString keeps the type, inner exceptions and the stack with file:line, so the message alone is complete.
                this.message = message + Environment.NewLine + exception.ToString();
                exceptionType = exception.GetType().FullName;
            }
            RaiseLogAdded(this);
        }

        /// <summary>
        /// Builds "File.Member:line" from caller information, or null when the compiler supplied none.
        /// </summary>
        /// <param name="member">The calling member.</param>
        /// <param name="file">The calling source file.</param>
        /// <param name="line">The calling line.</param>
        /// <returns>The operation name, or null.</returns>
        internal static string FormatOperationName(string member, string file, int line)
        {
            if (string.IsNullOrEmpty(member) || string.IsNullOrEmpty(file))
            {
                return null;
            }
            int slash = Math.Max(file.LastIndexOf('/'), file.LastIndexOf('\\'));
            string fileName = slash >= 0 ? file.Substring(slash + 1) : file;
            int dot = fileName.LastIndexOf('.');
            string stem = dot > 0 ? fileName.Substring(0, dot) : fileName;
            return line > 0 ? $"{stem}.{member}:{line}" : $"{stem}.{member}";
        }

        /// <summary>
        /// Raises <see cref="LogAdded"/> for each subscriber separately. A subscriber that throws is reported on
        /// <see cref="Console.Error"/> and never propagates into the code that merely logged.
        /// </summary>
        /// <param name="log">The new message.</param>
        private static void RaiseLogAdded(LogMessage log)
        {
            LogAddedEventHandler handlers = LogAdded;
            if (handlers == null)
            {
                return;
            }
            LogMessageEventArgs args = new LogMessageEventArgs(log);
            foreach (Delegate handler in handlers.GetInvocationList())
            {
                try
                {
                    ((LogAddedEventHandler)handler)(log, args);
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"[AnointedAutomation.Logging] LogAdded subscriber {handler.Method.DeclaringType?.FullName}.{handler.Method.Name} threw while handling a {log.messageType} log from {log.localOperationName}: {ex}");
                }
            }
        }

        // Define the delegate and event
        /// <summary>
        /// Delegate for handling log message events.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The log message event arguments.</param>
        public delegate void LogAddedEventHandler(object sender, LogMessageEventArgs e);

        /// <summary>
        /// Event triggered when a new log message is added. Each subscriber is invoked in isolation: an exception in
        /// one subscriber is written to <see cref="Console.Error"/> and does not reach the logging caller or other subscribers.
        /// </summary>
        public static event LogAddedEventHandler LogAdded;

        /// <summary>
        /// Gets or sets the default message source for all log messages.
        /// Default value indicates that the program name hasn't been set.
        /// </summary>
        public static string MessageSourceSetter { get; set; } = "You didn't set the name of your program!";

        /// <summary>
        /// Gets or sets the unique identifier for the log message.
        /// </summary>
        public long id { get; set; }
        /// <summary>
        /// Gets or sets the name of the operation that generated the log. From a factory it is "File.Member:line"
        /// (exact call site); from a typed constructor it is "Namespace.Class.Method" resolved from the stack.
        /// </summary>
        public string localOperationName { get; set; }
        /// <summary>
        /// Gets or sets the log message content. Exception overloads append the full exception text (type, message,
        /// inner exceptions and stack trace).
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// Gets or sets the source of the log message (typically the program name).
        /// </summary>
        public string messageSource { get; set; }
        /// <summary>
        /// Gets or sets the type of the log message (e.g., Error, Warning, Info).
        /// </summary>
        public MessageType messageType { get; set; }
        /// <summary>
        /// Gets or sets the UTC timestamp when the log message was created.
        /// </summary>
        public DateTime timeStamp { get; set; }
        /// <summary>
        /// Gets or sets the full type name of the exception passed to an exception overload, or null.
        /// </summary>
        public string exceptionType { get; set; }
        /// <summary>
        /// Gets or sets the calling member supplied by the compiler (factories only), or null.
        /// </summary>
        public string callerMemberName { get; set; }
        /// <summary>
        /// Gets or sets the calling source file supplied by the compiler (factories only), or null.
        /// </summary>
        public string callerFilePath { get; set; }
        /// <summary>
        /// Gets or sets the calling source line supplied by the compiler (factories only), or 0.
        /// </summary>
        public int callerLineNumber { get; set; }
        /// <summary>
        /// Gets or sets an optional logger category (for bridges such as an ILogger provider), or null.
        /// </summary>
        public string category { get; set; }
        /// <summary>
        /// Gets or sets an optional tenant or store identifier the log belongs to, or null.
        /// </summary>
        public string storeSlug { get; set; }
        /// <summary>
        /// Gets or sets optional structured properties (for example a JSON object of ids), or null.
        /// </summary>
        public string properties { get; set; }

        /// <summary>
        /// Creates a celebrate log message (MessageType.Celebrate) and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Celebrate.</returns>
        public static LogMessage Celebrate(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Celebrate, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a celebrate log message (MessageType.Celebrate) with an id and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="id">ID of the log message.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Celebrate.</returns>
        public static LogMessage Celebrate(long id, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(id, MessageType.Celebrate, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a critical log message (MessageType.Critical) and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Critical.</returns>
        public static LogMessage Critical(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Critical, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a critical log message (MessageType.Critical) with an id and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="id">ID of the log message.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Critical.</returns>
        public static LogMessage Critical(long id, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(id, MessageType.Critical, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a critical log message (MessageType.Critical) that carries the full exception (type, message, inner
        /// exceptions and stack trace with file:line) appended to <paramref name="message"/>, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">What failed and the ids needed to act on it.</param>
        /// <param name="exception">The exception; null logs the message alone.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Critical.</returns>
        public static LogMessage Critical(string message, Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Critical, message, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates an error log message (MessageType.Error) and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Error.</returns>
        public static LogMessage Error(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Error, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates an error log message (MessageType.Error) with an id and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="id">ID of the log message.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Error.</returns>
        public static LogMessage Error(long id, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(id, MessageType.Error, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates an error log message (MessageType.Error) that carries the full exception (type, message, inner
        /// exceptions and stack trace with file:line) appended to <paramref name="message"/>, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">What failed and the ids needed to act on it.</param>
        /// <param name="exception">The exception; null logs the message alone.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Error.</returns>
        public static LogMessage Error(string message, Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Error, message, exception, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates an informational log message (MessageType.Informational) and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Informational.</returns>
        public static LogMessage Info(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Informational, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates an informational log message (MessageType.Informational) with an id and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="id">ID of the log message.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Informational.</returns>
        public static LogMessage Info(long id, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(id, MessageType.Informational, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates an informational log message (MessageType.Informational) and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Informational.</returns>
        public static LogMessage Informational(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Informational, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates an informational log message (MessageType.Informational) with an id and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="id">ID of the log message.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Informational.</returns>
        public static LogMessage Informational(long id, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(id, MessageType.Informational, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a message log message (MessageType.Message) and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Message.</returns>
        public static LogMessage Message(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Message, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a message log message (MessageType.Message) with an id and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="id">ID of the log message.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Message.</returns>
        public static LogMessage Message(long id, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(id, MessageType.Message, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a success log message (MessageType.Success) and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Success.</returns>
        public static LogMessage Success(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Success, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a success log message (MessageType.Success) with an id and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="id">ID of the log message.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Success.</returns>
        public static LogMessage Success(long id, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(id, MessageType.Success, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a warning log message (MessageType.Warning) and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Warning.</returns>
        public static LogMessage Warning(string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Warning, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a warning log message (MessageType.Warning) with an id and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="id">ID of the log message.</param>
        /// <param name="message">The message.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Warning.</returns>
        public static LogMessage Warning(long id, string message, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(id, MessageType.Warning, message, null, callerMemberName, callerFilePath, callerLineNumber);
        }

        /// <summary>
        /// Creates a warning log message (MessageType.Warning) that carries the full exception (type, message, inner
        /// exceptions and stack trace with file:line) appended to <paramref name="message"/>, and raises <see cref="LogAdded"/>.
        /// </summary>
        /// <param name="message">What failed and the ids needed to act on it.</param>
        /// <param name="exception">The exception; null logs the message alone.</param>
        /// <param name="callerMemberName">Supplied by the compiler; do not pass.</param>
        /// <param name="callerFilePath">Supplied by the compiler; do not pass.</param>
        /// <param name="callerLineNumber">Supplied by the compiler; do not pass.</param>
        /// <returns>A LogMessage instance with MessageType.Warning.</returns>
        public static LogMessage Warning(string message, Exception exception, [CallerMemberName] string callerMemberName = "", [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            return new LogMessage(0, MessageType.Warning, message, exception, callerMemberName, callerFilePath, callerLineNumber);
        }
    }

    /// <summary>
    /// Event arguments for log message events.
    /// </summary>
    public class LogMessageEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessageEventArgs"/> class.
        /// </summary>
        /// <param name="logMessage">The log message associated with this event.</param>
        public LogMessageEventArgs(LogMessage logMessage)
        {
            log = logMessage;
        }

        /// <summary>
        /// Gets the log message associated with this event.
        /// </summary>
        public LogMessage log { get; }
    }
}
